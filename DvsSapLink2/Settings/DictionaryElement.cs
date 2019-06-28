using System.Configuration;

namespace DvsSapLink2.Settings
{
    /// <summary>
    /// Dictionary element to be used in app.config
    /// </summary>
    public class DictionaryElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name of the element
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        /// <summary>
        /// Gets or sets the value of the element
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get => (string)this["value"];
            set => this["value"] = value;
        }
    }
}