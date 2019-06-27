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
                    this.SourceDirectory = @"D:\Development\BirrMachines\testFiles\work\userwork";
                    this.DestinationDirectory = @"D:\Development\BirrMachines\testFiles\pre";
                    this.LogDirectory = @"D:\Development\BirrMachines\testFiles\work\log";
                    break;

                case ConfigurationType.Archive:
                    this.SourceDirectory = "D:\\Development\\BirrMachines\\test\\PreArchive\\";
                    this.DestinationDirectory = "D:\\Development\\BirrMachines\\test\\Archive\\";
                    this.LogDirectory = @"D:\Development\BirrMachines\testFiles\work\log";
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
        public bool CanChangeDestinationDirectory => this.Type == ConfigurationType.Archive;
      
        /// <summary>
        /// Gets the directory where to put the log files
        /// </summary>
        public string LogDirectory { get; }
    }
}