namespace DwgSapLink2.Model
{
    /// <summary>
    /// List of all possible file attributes
    /// </summary>
    public enum FileAttributeName
    {
        ZeichnungsNummer,
        Attribute_01
    }

    public class FileAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FileAttribute"/> class
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="rawValue">Raw value of the attribute</param>
        /// <param name="value">Processed value of the attribute</param>
        public FileAttribute(FileAttributeName name, string rawValue, string value)
        {
            this.Name = name;
            this.RawValue = rawValue;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        public FileAttributeName Name { get; }

        /// <summary>
        /// Gets the original value of the attribute
        /// </summary>
        public string RawValue { get; }

        /// <summary>
        /// Gets the processed value of the attribute
        /// </summary>
        public string Value { get; }
    }
}