using System;
using System.Collections.Generic;
using System.IO;
using DwgSapLink2.Model;

namespace DwgSapLink2.Helper
{
    public class FileAttributeParser
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
            { FileAttributeName.ZeichnungsNummer, new FileAttributeDefinition(0, 13, s => s.ToUpper()) },
            { FileAttributeName.Typ, new FileAttributeDefinition(13, 24) },
            { FileAttributeName.Haupttitel, new FileAttributeDefinition(37, 24) },
            { FileAttributeName.Untertitel, new FileAttributeDefinition(61, 24) },
            { FileAttributeName.AuftragsNummer, new FileAttributeDefinition(85, 24, FileAttributeParser.FormatOrderNumber) },
            { FileAttributeName.Ersteller, new FileAttributeDefinition(109, 22) },
            { FileAttributeName.Prüfer1, new FileAttributeDefinition(131, 22) },
            { FileAttributeName.Prüfer2, new FileAttributeDefinition(153, 22) },
            { FileAttributeName.Freigeber, new FileAttributeDefinition(175, 22) },
            { FileAttributeName.SLgleicherNr, new FileAttributeDefinition(197, 1) },
            { FileAttributeName.SLandererNr, new FileAttributeDefinition(198, 1) },
            { FileAttributeName.EntstandAus, new FileAttributeDefinition(199, 10) },
            { FileAttributeName.ErsatzFuer, new FileAttributeDefinition(209, 10) },
            { FileAttributeName.Massstab, new FileAttributeDefinition(219, 5) },
            { FileAttributeName.ZustandStelle, new FileAttributeDefinition(224, 8) },
            { FileAttributeName.UebernehmendeStelle, new FileAttributeDefinition(232, 8) },
            { FileAttributeName.DokumentArt, new FileAttributeDefinition(240, 3) },
            { FileAttributeName.Sprache, new FileAttributeDefinition(243, 2) },
            { FileAttributeName.Format, new FileAttributeDefinition(245, 2) },
            { FileAttributeName.BlattNr, new FileAttributeDefinition(247, 2) },
            { FileAttributeName.AnzBlatt, new FileAttributeDefinition(249, 2) },
            { FileAttributeName.ToleranzMittel, new FileAttributeDefinition(251, 1) },
            { FileAttributeName.ToleranzGrob, new FileAttributeDefinition(252, 1) },
            { FileAttributeName.AeStand_aktuell, new FileAttributeDefinition(253, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_1, new FileAttributeDefinition(267, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_2, new FileAttributeDefinition(281, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_3, new FileAttributeDefinition(295, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_4, new FileAttributeDefinition(309, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_5, new FileAttributeDefinition(323, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_6, new FileAttributeDefinition(337, 14, s => s.ToUpper()) },
            { FileAttributeName.AeStand_7, new FileAttributeDefinition(351, 14, s => s.ToUpper()) },
            { FileAttributeName.Bemerkung, new FileAttributeDefinition(365, 60) },
        };

        /// <summary>
        /// Parses the file content and creates <see cref="FileAttribute"/>'s for the found fields.
        /// </summary>
        /// <param name="filePath">File path containing the field data</param>
        /// <returns>Enumerable of the found file attributes</returns>
        public static IEnumerable<FileAttribute> Parse(string filePath)
        {
            var content = File.ReadAllText(filePath);
            foreach(var definition in FileAttributeParser.Definitions)
            {
                if (!definition.Value.TryParse(content, out var attribute))
                    continue;
                yield return new FileAttribute(definition.Key, attribute?.RawValue, attribute?.Value);
            }
        }
        
        private static string FormatOrderNumber(string value)
        {
            // TODO: convert order number
            return value;
        }
    }
}