namespace DvsSapLink2.Model
{
    /// <summary>
    /// List of supported archive configurations. This has impact on the default value and supported actions
    /// </summary>
    public enum ConfigurationType
    {
        Prepare,
        Archive
    }

    public class Configuration
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Configuration"/> class
        /// </summary>
        /// <param name="type">Type of the configuration</param>
        public Configuration(ConfigurationType type)
        {
            this.Type = type;
            switch (type)
            {
                case ConfigurationType.Prepare:
                    this.SourceDirectory = Properties.Settings.Default.WORKING_DIRECTORY;
                    this.DestinationDirectory = Properties.Settings.Default.ARCHIVE_DWG_DIRECTORY;
                    this.PendingDirectory = Properties.Settings.Default.PENDING_DIRECTORY;
                    this.LogDirectory = Properties.Settings.Default.ARCHIVE_LOG_DIRECTORY;
                    this.ArchiveTifDirectory = Properties.Settings.Default.ARCHIVE_TIF_DIRECTORY;
                    break;

                case ConfigurationType.Archive:
                    this.SourceDirectory = Properties.Settings.Default.PENDING_DIRECTORY;
                    this.DestinationDirectory = Properties.Settings.Default.ARCHIVE_DWG_DIRECTORY;
                    this.ConversionDirectory = Properties.Settings.Default.CONVERSION_DIRECTORY;
                    this.TxtDirectory = Properties.Settings.Default.ARCHIVE_TXT_DIRECTORY;
                    this.LogDirectory = Properties.Settings.Default.ARCHIVE_LOG_DIRECTORY;
                    this.SapTransferDirectory = Properties.Settings.Default.SAPTRANSFER_DIRECTORY;
                    this.ArchiveSapTransferDirectory = Properties.Settings.Default.ARCHIVE_SAPTRANSFER_DIRECTORY;
                    this.ArchiveTifDirectory = Properties.Settings.Default.ARCHIVE_TIF_DIRECTORY;
                    break;
            }
        }

        /// <summary>
        /// Gets the current configuration type
        /// </summary>
        public ConfigurationType Type { get; }

        /// <summary>
        /// Gets or sets the source directory of the archive operation
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source directory can be changed or not
        /// </summary>
        public bool CanChangeSourceDirectory => this.Type == ConfigurationType.Prepare;

        /// <summary>
        /// Gets or sets the destination directory for the archive operation
        /// </summary>
        public string DestinationDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the destination directory can be changed or not
        /// </summary>
        public bool CanChangeDestinationDirectory => this.Type == ConfigurationType.Prepare;

        /// <summary>
        /// Gets the directory where to put the files (dwg, txt, pdf) for preparation of archiving
        /// </summary>
        public string PendingDirectory { get; }

        /// <summary>
        /// Gets the directory where to put the dwg to be converted to pdf and tif (observed folder)
        /// </summary>
        public string ConversionDirectory { get; }

        /// <summary>
        /// Gets the directory where to put the log files
        /// </summary>
        public string LogDirectory { get; }

        /// <summary>
        /// Gets the directory where to put the txt file to be archived
        /// </summary>
        public string TxtDirectory { get; }

        /// <summary>
        /// Gets the directory where to put the 'transfer file for SAP' (observed folder for FTP-Transfer to SAP)
        /// </summary>
        public string SapTransferDirectory { get; }

        /// <summary>
        /// Gets the directory where to put the 'transfer file for SAP' to be archived
        /// </summary>
        public string ArchiveSapTransferDirectory { get; }

        /// <summary>
        /// Gets the directory where to put the 'transfer file for SAP' to be archived
        /// </summary>
        public string ArchiveTifDirectory { get; }
    }
}