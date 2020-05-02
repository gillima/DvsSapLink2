using System.Configuration;

namespace DvsSapLink2.Settings
{
    /// <summary>
    /// Configuration class to be used in app.config to configure the values for the Input lists
    /// </summary>
    public class GuiSettings : ConfigurationSection
    {
        private SettingsDictionary states;
        private SettingsDictionary atexs;
        private SettingsDictionary orderstates;
        private SettingsDictionary classifications;
        private SettingsDictionary doccontents;
        private SettingsDictionary users;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiSettings"/> class
        /// </summary>
        public GuiSettings()
        {
            this.states = new SettingsDictionary();
            this.atexs = new SettingsDictionary();
            this.orderstates = new SettingsDictionary();
            this.classifications = new SettingsDictionary();
            this.doccontents = new SettingsDictionary();
            this.users = new SettingsDictionary();
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

        /// <summary>
        /// Gets or sets the Atexs
        /// </summary>
        [ConfigurationProperty("atexs", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary Atexs
        {
            get => (SettingsDictionary)this["atexs"];
            set => this["atexs"] = value;
        }

        /// <summary>
        /// Gets or sets the OrderStates
        /// </summary>
        [ConfigurationProperty("orderstates", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary OrderStates
        {
            get => (SettingsDictionary)this["orderstates"];
            set => this["orderstates"] = value;
        }

        /// <summary>
        /// Gets or sets the Classifications
        /// </summary>
        [ConfigurationProperty("classifications", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary Classifications
        {
            get => (SettingsDictionary)this["classifications"];
            set => this["classificatins"] = value;
        }

        /// <summary>
        /// Gets or sets the DocContents
        /// </summary>
        [ConfigurationProperty("doccontents", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary DocContents
        {
            get => (SettingsDictionary)this["doccontents"];
            set => this["doccontents"] = value;
        }

        /// <summary>
        /// Gets or sets the Usesrs
        /// </summary>
        [ConfigurationProperty("users", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DictionaryElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SettingsDictionary Users
        {
            get => (SettingsDictionary)this["users"];
            set => this["users"] = value;
        }
    }
}