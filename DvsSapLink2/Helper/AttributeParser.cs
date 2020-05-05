using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using DvsSapLink2.Model;
using DvsSapLink2.Resources;
using DvsSapLink2.ViewModel;
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
            { FileAttributeName.BlattFormat, new FileAttributeDefinition(245, 2, FormatSheetForm) },
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
            { FileAttributeName.ATEX, GetSapAtex },
            { FileAttributeName.Auftragsstatus, GetSapOrderState },
            { FileAttributeName.Klassifizierung, GetSapClassification },
            // Kundenauftrag
            { FileAttributeName.Projektname, GetSapProjectName },
            // Typ
            { FileAttributeName.CadApp, (f,s) => "AutoCAD" },
            // Format
            { FileAttributeName.DokInhalt, GetSapDocContent },
            // Ersatz für
            { FileAttributeName.ErsetztDurch, (f,s) => string.Empty },
            // ErnstandAus
            { FileAttributeName.EinordnungsNr, (f,s) => string.Empty },
            { FileAttributeName.ErstelltNameELO, BuildUserNameCreated },
            { FileAttributeName.GeprueftNameELO, BuildUserNameApproved },
            { FileAttributeName.FreigegebenNameELO, BuildUserNameReleased },
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

        private static string GetSapAtex(AttributeFile file, SapData sapData)
        {
            return $"{sapData.Atex}";
        }

        private static string GetSapOrderState(AttributeFile file, SapData sapData)
        {
            return $"{sapData.OrderState}";
        }

        private static string GetSapClassification(AttributeFile file, SapData sapData)
        {
            return $"{sapData.Classification}";
        }

        private static string GetSapProjectName(AttributeFile file, SapData sapData)
        {
            return $"{sapData.Project}";
        }

        private static string GetSapDocContent(AttributeFile file, SapData sapData)
        {
            return $"{sapData.DocContent}";
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
            return !String.IsNullOrEmpty(file[FileAttributeName.Untertitel].Trim())
                ? $"{file[FileAttributeName.Haupttitel]} / {file[FileAttributeName.Untertitel]}"
                : file[FileAttributeName.Haupttitel];
        }

        private static string BuildUnterordner(AttributeFile file, SapData sapData)
        {
            return $"{file[FileAttributeName.Zeichnungsnummer].Substring(0, 4).Trim()}";
        }

        private static string BuildUserNameCreated(AttributeFile file, SapData sapData)
        {
            return GetEloUserFromAppConfig(file[FileAttributeName.ErstelltName], sapData);
        }

        private static string BuildUserNameApproved(AttributeFile file, SapData sapData)
        {
            return GetEloUserFromAppConfig(file[FileAttributeName.GeprueftName], sapData);
        }

        private static string BuildUserNameReleased(AttributeFile file, SapData sapData)
        {
            return GetEloUserFromAppConfig(file[FileAttributeName.FreigegebenName], sapData);
        }

        private static string GetEloUserFromAppConfig(string attributeUser, SapData sapData)
        {
            // Users-Liste aus dem Settings-File lesen und Namen finden, der dem ErstelltName entspricht 
            // (vorläufig erste drei Zeichen des Nachnamens, max. 8 möglich)

            var match = Regex.Match(attributeUser.ToLower(), "^([a-z]*\\b)[. ]*(\\w.*)$");

            // string attributeUserPart1 = match.Success
            //     ? match.Groups[1].Value.Substring(0, 1)
            //     : string.Empty;
            string attributeUserPart2 = match.Success
                ? match.Groups[2].Value.Substring(0, 3)
                : string.Empty;

            foreach (var entry in sapData.Users)
            {
                match = Regex.Match(entry.Key.ToLower(), "^([a-z]*\\b)[. ]*(\\w.*)$");
                string eloUserPart2 = match.Success
                    ? match.Groups[2].Value.Substring(0, 3)
                    : string.Empty;

                if (eloUserPart2 == attributeUserPart2)
                {
                    return entry.Value;
                }
            }
            return attributeUser;
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

        private static string FormatSheetForm(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // regex returns first A-character and following numbers
                var match = Regex.Match(value, "([A]\\d{1,2}$)");
                value = match.Groups[1].Value;
            }
            return value;
        }

        private static string GetDate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // regex returns any characters from the start until to the spaces
                var match = Regex.Match(value, "^([^ ]*)[ ]+(.*)$");
                value = match.Groups[1].Value;
            }
            return value;
        }

        private static string GetName(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // regex returns any characters after the spaces until the end (example string: "2018-04-30 I. Walt")
                var match = Regex.Match(value, "^([^ ]*)[ ]+(.*)$");
                value = match.Groups[2].Value;

                value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
            }
            return value;
        }

        private static string GetLanguageCode(string value)
        {
            // regex retruns one character D, E or / from possible content [D, E, D/]
            var match = Regex.Match(value, "([D,E,\\/])$");
            value = match.Groups[1].Value;
            return $"{value.Replace("/", "xx").Replace("E", "en").Replace("D", "de")}";
        }

        private static string GetSheetNumber(string value)
        {
            return int.TryParse(value.Trim(' ', '/'), out var number)
                ? $"{number,2:00}"
                : value; // wenn es keine Zahl ist, wird in AttributeFileViewModel abgefangen
        }
    }
}
