using System.Configuration;

namespace DvsSapLink2.Settings
{
    /// <summary>
    /// Configuration class to be used in app.config to configure the values for the Labors and States lists
    /// </summary>
    public class GuiSettings : ConfigurationSection
    {
        private SettingsDictionary labors;
        private SettingsDictionary states;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiSettings"/> class
        /// </summary>
        public GuiSettings()
        {
            this.labors = new SettingsDictionary();
            this.states = new SettingsDictionary();
        }

        /// <summary>
        /// Gets or sets the Labors
        /// </summary>
        [ConfigurationProperty("labors", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary Labors
        {
            get => (SettingsDictionary)this["labors"];
            set => this["labors"] = value;
        }

        /// <summary>
        /// Gets or sets the States
        /// </summary>
        [ConfigurationProperty("states", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary States
        {
            get => (SettingsDictionary)this["states"];
            set => this["states"] = value;
        }
    }
}