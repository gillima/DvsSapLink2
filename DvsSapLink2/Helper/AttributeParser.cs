﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DvsSapLink2.Model;
using DvsSapLink2.Resources;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.Helper
{
    public static class FileAttributeParser
    {
        /// <summary>
        /// Internal helper class used by the file attribute parser to parse the fields
        /// </summary>
        private class FileAttributeDefinition
        {
            private readonly int Start;
            private readonly int Length;
            private readonly Func<string, string> Transform;

            public FileAttributeDefinition(int start, int length, Func<string, string> transform = null)
            {
                this.Start = start;
                this.Length = length;
                this.Transform = transform ?? new Func<string, string>(value => value);
            }

            public bool TryParse(string content, out (string RawValue, string Value)? attribute)
            {
                if (content.Length <= this.Start)
                {
                    attribute = (null, string.Empty);
                    return false;
                }

                var rawValue = content.Substring(this.Start, this.Length);
                var value = this.Transform(rawValue.Trim());
                attribute = (rawValue, value);
                return !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Definition of the file attributes for the parser
        /// </summary>
        private static readonly IDictionary<FileAttributeName, FileAttributeDefinition> Definitions = new Dictionary<FileAttributeName, FileAttributeDefinition>
        {
            { FileAttributeName.Zeichnungsnummer, new FileAttributeDefinition(0, 13, s => s.ToUpper()) },
            { FileAttributeName.Typ, new FileAttributeDefinition(13, 24) },
            { FileAttributeName.Haupttitel, new FileAttributeDefinition(37, 24) },
            { FileAttributeName.Untertitel, new FileAttributeDefinition(61, 24) },
            { FileAttributeName.AuftragsNummer, new FileAttributeDefinition(85, 24, FormatOrderNumber) },
            { FileAttributeName.ErstelltDatum, new FileAttributeDefinition(109, 22, GetDate) },
            { FileAttributeName.ErstelltName, new FileAttributeDefinition(109, 22, GetName) },
            { FileAttributeName.GeprueftDatum, new FileAttributeDefinition(131, 22, GetDate) },
            { FileAttributeName.GeprueftName, new FileAttributeDefinition(131, 22, GetName) },
            // { FileAttributeName.Prüfer2, new FileAttributeDefinition(153, 22) },
            { FileAttributeName.FreigegebenDatum, new FileAttributeDefinition(175, 22, GetDate) },
            { FileAttributeName.FreigegebenName, new FileAttributeDefinition(175, 22, GetName) },
            // { FileAttributeName.SLgleicherNr, new FileAttributeDefinition(197, 1) },
            // { FileAttributeName.SLandererNr, new FileAttributeDefinition(198, 1) },
            { FileAttributeName.EntstandAus, new FileAttributeDefinition(199, 10) },
            { FileAttributeName.ErsatzFuer, new FileAttributeDefinition(209, 10) },
            // { FileAttributeName.Massstab, new FileAttributeDefinition(219, 5) },
            // { FileAttributeName.ZustandStelle, new FileAttributeDefinition(224, 8) },
            // { FileAttributeName.UebernehmendeStelle, new FileAttributeDefinition(232, 8) },
            // { FileAttributeName.DokumentArt, new FileAttributeDefinition(240, 3) },
            { FileAttributeName.Sprache, new FileAttributeDefinition(243, 2, GetLanguageCode) },
            { FileAttributeName.BlattFormat, new FileAttributeDefinition(245, 2) },
            { FileAttributeName.BlattNr, new FileAttributeDefinition(247, 2, GetSheetNumber) },
            // { FileAttributeName.AnzBlatt, new FileAttributeDefinition(249, 2) },
            // { FileAttributeName.ToleranzMittel, new FileAttributeDefinition(251, 1) },
            // { FileAttributeName.ToleranzGrob, new FileAttributeDefinition(252, 1) },
            { FileAttributeName.AeStand_aktuell, new FileAttributeDefinition(253, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_1, new FileAttributeDefinition(267, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_2, new FileAttributeDefinition(281, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_3, new FileAttributeDefinition(295, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_4, new FileAttributeDefinition(309, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_5, new FileAttributeDefinition(323, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_6, new FileAttributeDefinition(337, 14, s => s.ToUpper()) },
            // { FileAttributeName.AeStand_7, new FileAttributeDefinition(351, 14, s => s.ToUpper()) },
            // { FileAttributeName.Bemerkung, new FileAttributeDefinition(365, 60) },
        };

        /// <summary>
        /// Converter definitions to enhance the attributes of the file
        /// </summary>
        public static readonly IDictionary<FileAttributeName, Func<AttributeFile, SapData, string>> ConvertDefinitions = new Dictionary<FileAttributeName, Func<AttributeFile, SapData, string>>
        {
            // TODO: Add new fields....
            { FileAttributeName.Dateiname, BuildDateiname },
            { FileAttributeName.Dateiversion, (f,s) => "1.0" },
            { FileAttributeName.SapID, (f,s) => string.Empty },
            { FileAttributeName.Unterordner1, BuildUnterordner },
            { FileAttributeName.Kurzbezeichnung, BuildKurzbezeichnung },
            { FileAttributeName.DokDatum, (f,s) => string.Empty },
            { FileAttributeName.Titel, BuildFullTitle },
            // ZeichnungsNummer
            { FileAttributeName.DokStatus, (f,s) => "freigegeben" },
            // AeStand_aktuell
            // BlattNr
            { FileAttributeName.DokTyp, (f,s) => "ZE" },
            // Sprache
            { FileAttributeName.FertigungsProzess, (f,s) => string.Empty },
            { FileAttributeName.StandUeberarbeitung, (f,s) => string.Empty },
            { FileAttributeName.Verteilung, (f,s) => "LAUF" },
            { FileAttributeName.ATEX, (f,s) => "xxx" },
            { FileAttributeName.Auftragsstatus, (f,s) => "xxx" },
            { FileAttributeName.Klassifizierung, (f,s) => "xxx" },
            // Kundenauftrag
            { FileAttributeName.Projektname, (f,s) => "xxx" },
            // Typ
            { FileAttributeName.CadApp, (f,s) => "AutoCAD" },
            // Format
            { FileAttributeName.DokInhalt, (f,s) => "xxx" },
            // Ersatz für
            { FileAttributeName.ErsetztDurch, (f,s) => string.Empty },
            // ErnstandAus
            { FileAttributeName.EinordnungsNr, (f,s) => string.Empty },
            // 3x Datum und Name
        };

        /// <summary>
        /// Parses the file content and creates <see cref="FileAttribute"/>'s for the found fields.
        /// </summary>
        /// <param name="filePath">File path containing the field data</param>
        /// <returns>Enumerable of the found file attributes</returns>
        public static IEnumerable<FileAttribute> Parse(string filePath)
        {
            var content = File.ReadAllText(filePath);

            var values = new Dictionary<FileAttributeName, string>();
            foreach (var definition in FileAttributeParser.Definitions)
            {
                definition.Value.TryParse(content, out var attribute);
                yield return new FileAttribute(definition.Key, attribute?.RawValue, attribute?.Value);
            }
        }

        private static string BuildDateiname(AttributeFile file, SapData sapData)
        {
            return $"{file.Title}.pdf";
        }

        private static string BuildKurzbezeichnung(AttributeFile file, SapData sapData)
        {
            return file.Title;
        }

        private static string BuildFullTitle(AttributeFile file, SapData sapData)
        {
            return file[FileAttributeName.Untertitel] != null
                ? $"{file[FileAttributeName.Haupttitel]} / {file[FileAttributeName.Untertitel]}"
                : file[FileAttributeName.Haupttitel];
        }

        private static string BuildUnterordner(AttributeFile file, SapData sapData)
        {
            return $"{file[FileAttributeName.Zeichnungsnummer].Substring(0, 4).Trim()}";
        }

        private static string FormatOrderNumber(string value)
        {
            // z.B. 0011005084* -> 11005084*, die führenden Nullen sollen entfernt werden (egal, ob noch eine -Pos kommt oder nicht)
            var match = Regex.Match(value, "^00([0-9]{8}[-]?[0-9]*$)");
            if (match.Success)
                value = match.Groups[1].Value;

            // z.B. 1-1005084* -> 11005084*, Trennstrich entfernen wenn danach Zahl 8-stellig
            match = Regex.Match(value, "^([1-9])-([0-9]{7}[-]?.*)$");
            if (match.Success)
                value = match.Groups[1].Value + match.Groups[2].Value;

            // z.B. 11005084-101 -> 11005084-000101, Position soll 6-stellig sein für 8-stellige Auftragsnummern
            match = Regex.Match(value, "(^[0-9]{8}-)([0]{0,3})([0-9]{3})$");
            if (match.Success)
                value = match.Groups[1].Value + $"000" + match.Groups[3].Value;

            return value.Trim();
        }

        private static string GetDate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // regex returns any characters from the start until the spaces
                var match = Regex.Match(value, "^([^ ]*)[ ]+(.*)$");
                value = match.Groups[1].Value;

                // TODO: Datum so formatieren, dass es in ELO richtig als Datum importiert wird
            }
            return value;
        }

        private static string GetName(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // regex returns any characters after the spaces until the end
                var match = Regex.Match(value, "^([^ ]*)[ ]+(.*)$");
                value = match.Groups[2].Value;
            }
            return value;
        }

        private static string GetLanguageCode(string value)
        {
            return $"{value.Replace("E", "en").Replace("D", "de")}";
        }

        private static string GetSheetNumber(string value)
        {
            return int.TryParse(value.Trim(' ', '/'), out var number)
                ? $"{number,2:00}"
                : value; // wenn es keine Zahl ist, wird in AttributeFileViewModel abgefangen
        }
    }
}
