using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using DvsSapLink2.Command;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using GalaSoft.MvvmLight;

namespace DvsSapLink2.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly Configuration configuration;
        private readonly ObservableCollection<AttributeFileViewModel> files;

        /// <summary>
        /// Creates a new instance of the <see cref="ConfigurationViewModel"/> class
        /// </summary>
        /// <param name="configuration">DTO containing the configuration of the application</param>
        /// <param name="files">List of available files in the current source directory</param>
        public ConfigurationViewModel(Configuration configuration, ObservableCollection<AttributeFileViewModel> files)
        {
            this.configuration = configuration;
            this.files = files;

            this.ArchiveCommand = configuration.Type == ConfigurationType.Archive
                ? (CopyCommand)new ArchiveCommand(this.configuration)
                : (CopyCommand)new PrepareCommand(this.configuration);
            this.SelectSourceDirectory = new BrowseDirectoryCommand(dir => this.SourceDirectory = dir, () => this.SourceDirectory);
            this.SelectDestinationDirectory = new BrowseDirectoryCommand(dir => this.DestinationDirectory = dir, () => this.DestinationDirectory);
            
            this.UpdateFiles();
        }

        /// <summary>
        /// Gets the configuration presented by this view model
        /// </summary>
        public Configuration Configuration => this.configuration;

        /// <summary>
        /// Gets the command to select the source directory
        /// </summary>
        public ICommand SelectSourceDirectory { get; }

        /// <summary>
        /// Gets the command to select the destination directory
        /// </summary>
        public ICommand SelectDestinationDirectory { get; }

        /// <summary>
        /// Gets the current configuration type
        /// </summary>
        public ConfigurationType Type => this.configuration.Type;

        /// <summary>
        /// Gets or sets the source directory of the archive operation
        /// </summary>
        public string SourceDirectory
        {
            get => this.configuration.SourceDirectory;
            set
            {
                if (this.configuration.CanChangeSourceDirectory)
                    this.configuration.SourceDirectory = value;
                this.RaisePropertyChanged();
                this.UpdateFiles();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the source directory can be changed or not
        /// </summary>
        public bool CanChangeSourceDirectory => this.configuration.CanChangeSourceDirectory;

        /// <summary>
        /// Gets or sets the destination directory for the archive operation
        /// </summary>
        public string DestinationDirectory
        {
            get => this.configuration.DestinationDirectory;
            set
            {
                if (this.configuration.CanChangeDestinationDirectory)
                    this.configuration.DestinationDirectory = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the destination directory can be changed or not
        /// </summary>
        public bool CanChangeDestinationDirectory => this.configuration.CanChangeDestinationDirectory;

        /// <summary>
        /// Gets the action to be performed when the archive button is pressed
        /// </summary>
        public CopyCommand ArchiveCommand { get; }

        /// <summary>
        /// Updates the list of available attribute files to show the content of the current source directory
        /// </summary>
        private void UpdateFiles()
        {
            this.files.Clear();
            if (!Directory.Exists(this.SourceDirectory)) return;
            foreach(var filePath in Directory.EnumerateFiles(this.SourceDirectory, "*.txt"))
            {
                var file = new AttributeFile(filePath);
                this.files.Add(new AttributeFileViewModel(file));
            }
        }
    }
}