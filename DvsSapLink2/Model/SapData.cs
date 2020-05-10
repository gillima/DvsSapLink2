using System.Collections.Generic;
using System.Configuration;
using DvsSapLink2.Settings;

namespace DvsSapLink2.Model
{
    /// <summary>
    /// Data Transfer Object (DTO) for the SAP data related to the documents to archive
    /// </summary>
    public class SapData
    {
        public SapData(ConfigurationType type)
        {
            var section = (GuiSettings)ConfigurationManager.GetSection("guiSettings");
            this.Users = section.Users.As<string, string>();
            this.Type = type;
            switch (type)
            {
                case ConfigurationType.Prepare:
                    this.User = Properties.Settings.Default.DEFAULT_SAP_USER_PREPARE;
                    this.State = Properties.Settings.Default.DEFAULT_SAP_STATE_PREPARE;
                    this.Atex = Properties.Settings.Default.DEFAULT_SAP_ATEX_PREPARE;
                    this.OrderState = Properties.Settings.Default.DEFAULT_SAP_ORDERSTATE_PREPARE;
                    this.Classification = Properties.Settings.Default.DEFAULT_SAP_CLASSIFICATION_PREPARE;
                    this.DocContent = Properties.Settings.Default.DEFAULT_SAP_DOCCONTENT_PREPARE;
                    break;

                case ConfigurationType.Archive:
                    // TODO: Read from log file
                    this.User = "";
                    this.State = "";
                    this.Atex = ""; 
                    this.OrderState = ""; 
                    this.Classification = ""; 
                    this.DocContent = ""; 
                    break;
            }
        }

        /// <summary>
        /// Gets the current configuration type
        /// </summary>
        public ConfigurationType Type { get; }

        /// <summary>
        /// Gets or sets the SAP user
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets the list of users for the selection box
        /// </summary>
        public IDictionary<string, string> Users { get; }
        
        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the ATEX state
        /// </summary>
        public string Atex { get; set; }

        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string OrderState { get; set; }

        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// Gets or sets the project name
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string DocContent { get; set; }
    }
}