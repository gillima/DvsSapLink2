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
            { FileAttributeName.ZeichnungsNummer, new FileAttributeDefinition(0, 15, s => s.ToUpperInvariant()) },
            { FileAttributeName.Attribute_01, new FileAttributeDefinition(15, 8) },
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
    }
}