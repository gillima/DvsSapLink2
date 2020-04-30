using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DvsSapLink2.Model;

namespace DvsSapLink2.Helper
{
    public static class FileAttributeParser
    {
        /// <summary>
        /// Internal helper class used by the file attribute parser to parse the fields
        /// </summary>
        private class FileAttributeDefinition
        {
            public FileAttributeDefinition(int start, int length, Func<string, string> transform = null)
            {
                this.Start = start;
                this.Length = length;
                this.Transform = transform ?? new Func<string, string>(value => value);
            }

            public int Start { get; }
            public int Length { get; }
            public Func<string, string> Transform { get; }

            public bool TryParse(string content, out (string RawValue, string Value)? attribute)
            {
                // TODO: neu müssen alle Attribute ausgegeben werden, auch wenn sie einen leeren String als Wert enthalten
                attribute = null;
                if (content.Length <= this.Start) return false;
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
        public static readonly IDictionary<FileAttributeName, Func<AttributeFile, SapData, (int Order, string Value)>> ConvertDefinitions = new Dictionary<FileAttributeName, Func<AttributeFile, SapData, (int Order, string Value)>>
        {
            // TODO: Add new fields....
            { FileAttributeName.Dateiname, BuildDateiname },
            { FileAttributeName.Dateiversion, (f,s) => (34, "1.0") },
            { FileAttributeName.SapID, (f,s) => (34, "") },
            { FileAttributeName.Unterordner1, BuildUnterordner },
            { FileAttributeName.Kurzbezeichnung, BuildKurzbezeichnung },
            { FileAttributeName.DokDatum, (f,s) => (34, "") },
            { FileAttributeName.Titel, BuildFullTitle },
            // ZeichnungsNummer
            { FileAttributeName.DokStatus, (f,s) => (34, "freigegeben") },
            // AeStand_aktuell
            // BlattNr
            { FileAttributeName.DokTyp, (f,s) => (34, "ZE") },
            // Sprache
            { FileAttributeName.FertigungsProzess, (f,s) => (34, "") },
            { FileAttributeName.StandUeberarbeitung, (f,s) => (34, "") },
            { FileAttributeName.Verteilung, (f,s) => (34, "LAUF") },
            { FileAttributeName.ATEX, (f,s) => (34, "xxx") },
            { FileAttributeName.Auftragsstatus, (f,s) => (34, "xxx") },
            { FileAttributeName.Klassifizierung, (f,s) => (34, "xxx") },
            // Kundenauftrag
            { FileAttributeName.Projektname, (f,s) => (34, "xxx") },
            // Typ
            { FileAttributeName.CadApp, (f,s) => (34, "AutoCAD") },
            // Format
            { FileAttributeName.DokInhalt, (f,s) => (34, "xxx") },
            // Ersatz für
            { FileAttributeName.ErsetztDurch, (f,s) => (34, "") },
            // ErnstandAus
            { FileAttributeName.EinordnungsNr, (f,s) => (34, "") },
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
                // Test, ob auch leere Strings ausgegeben werden können...
                //if (!definition.Value.TryParse(content, out var attribute))
                //    continue;
                definition.Value.TryParse(content, out var attribute);
                yield return new FileAttribute(definition.Key, attribute?.RawValue, attribute?.Value);
            }
        }

        private static (int Order, string Value) BuildDateiname(AttributeFile file, SapData sapData)
        {
            // TODO: richtig schreiben ?!
            // return (33, $"{String.Join(".", file.Title, "pdf")}");
            return (33, $"{file.Title}" + $".pdf" );
        }

        private static (int Order, string Value) BuildKurzbezeichnung(AttributeFile file, SapData sapData)
        {
            // TODO: richtig schreiben ?!
            return (33, $"{file.Title}");
        }

        private static (int Order, string Value) BuildFullTitle(AttributeFile file, SapData sapData)
        {
            // TODO: / nur wenn Untertitel vorhanden
            return (33, $"{file[FileAttributeName.Haupttitel]} / {file[FileAttributeName.Untertitel]}");
        }

        private static (int Order, string Value) BuildUnterordner(AttributeFile file, SapData sapData)
        {
            return (33, $"{file[FileAttributeName.Zeichnungsnummer].Substring(0, 4).Trim()}");
        }

        private static string FormatOrderNumber(string value)
        {
            // TODO: convert order number
            return value;
        }

        private static string GetDate(string value)
        {
            return $"{value.Substring(0, 10)}";
        }

        private static string GetName(string value)
        {
            return value.Substring(10).Trim();
        }

        private static string GetLanguageCode(string value)
        {
            return $"{value.Replace("E", "en").Replace("D", "de")}";
        }

        private static string GetSheetNumber(string value)
        {
            value = String.Format("{0,2:00}", value.Trim(' ', '/'));
            return value;
        }
    }
}
