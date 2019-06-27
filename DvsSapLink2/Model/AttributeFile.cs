using System.Collections.Generic;
using DvsSapLink2.Helper;

namespace DvsSapLink2.Model
{
    public class AttributeFile
    {
        private IEnumerable<FileAttribute> attributes;

        /// <summary>
        /// Creates a new instance of the <see cref="AttributeFile"/> class
        /// </summary>
        /// <param name="path">Full qualified path to the file</param>
        public AttributeFile(string path)
        {
            this.Path = path;
            this.Title = System.IO.Path
                .GetFileNameWithoutExtension(path)?
                .ToUpperInvariant();
        }

        /// <summary>
        /// Gets the full qualified path to the file
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the title of the file (uppercase filename without path and extension)
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the attributes of the file. The attributes are lazy loaded when the
        /// Attributes collection is accessed
        /// </summary>
        public IEnumerable<FileAttribute> Attributes
        {
            get
            {
                if (this.attributes != null)
                    return this.attributes;

                this.attributes = FileAttributeParser.Parse(this.Path);
                return this.attributes;
            }
        }
    }
}