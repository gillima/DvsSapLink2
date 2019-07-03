namespace DvsSapLink2.Model
{
    /// <summary>
    /// Data Transfer Object (DTO) for the SAP data related to the documents to archive
    /// </summary>
    public class SapData
    {
        public SapData(ConfigurationType type)
        {
            this.Type = type;
            switch (type)
            {
                case ConfigurationType.Prepare:
                    this.Labor = Properties.Settings.Default.DEFAULT_SAP_LABOR;
                    this.User = Properties.Settings.Default.DEFAULT_SAP_USER_PREPARE;
                    this.State = Properties.Settings.Default.DEFAULT_SAP_STATE_PREPARE;
                    break;

                case ConfigurationType.Archive:
                    this.Labor = Properties.Settings.Default.DEFAULT_SAP_LABOR;
                    this.User = Properties.Settings.Default.DEFAULT_SAP_USER_ARCHIVE;
                    this.State = Properties.Settings.Default.DEFAULT_SAP_STATE_ARCHIVE;
                    break;
            }
        }

        /// <summary>
        /// Gets the current configuration type
        /// </summary>
        public ConfigurationType Type { get; }

        /// <summary>
        /// Gets or sets the labor
        /// </summary>
        public int Labor { get; set; }

        /// <summary>
        /// Gets or sets the SAP user
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string State { get; set; }
    }
}