namespace DvsSapLink2.Model
{
    /// <summary>
    /// Data Transfer Object (DTO) for the SAP data related to the documents to archive
    /// </summary>
    public class SapData
    {
        public SapData()
        {
            this.Labor = Properties.Settings.Default.DEFAULT_SAP_LABOR;
            this.User = Properties.Settings.Default.DEFAULT_SAP_USER;
            this.State = Properties.Settings.Default.DEFAULT_SAP_STATE;
        }

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