using System.Collections.ObjectModel;
using System.Windows.Input;
using DvsSapLink2.Model;
using GalaSoft.MvvmLight;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string message;
        private AttributeFileViewModel file;

        /// <summary>
        /// Creates a new instance of the <see cref="MainViewModel"/> class
        /// </summary>
        /// <param name="configuration">Configuration to use</param>
        /// <param name="sapData">SAP data DTO</param>
        public MainViewModel(Configuration configuration, SapData sapData)
        {
            this.Configuration = new ConfigurationViewModel(configuration, this.Files);
            this.Sap = new SapDataViewModel(sapData);
            this.File = null;
            this.Archive.CanExecuteChanged += delegate { this.RaisePropertyChanged(nameof(MainViewModel.IsValid)); };
        }

        /// <summary>
        /// Gets the action to be performed when the archive button is pressed
        /// </summary>
        public ICommand Archive => this.Configuration.ArchiveCommand;

        /// <summary>
        /// Gets or sets a message to display in the status bar of the application
        /// </summary>
        public string Message
        {
            get => this.message;
            set => this.Set(ref this.message, value);
        }

        /// <summary>
        /// Gets the view model for the current application configuration
        /// </summary>
        public ConfigurationViewModel Configuration { get; }

        /// <summary>
        /// Gets the list of available attribute files in the selected directory
        /// </summary>
        public ObservableCollection<AttributeFileViewModel> Files { get; } = new ObservableCollection<AttributeFileViewModel>();

        /// <summary>
        /// Gets the view model of the currently selected attribute file
        /// </summary>
        public AttributeFileViewModel File
        {
            get => this.file;
            set
            {
                this.Set(ref this.file, value);
                this.RaisePropertyChanged(nameof(MainViewModel.IsValid));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current file selection is valid for copy
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (this.file == null)
                {
                    this.Message = TXT_NO_FILE_SELECTED;
                    return false;
                }

                if (!this.file.IsValid)
                {
                    this.Message = this.file.Message;
                    return false;
                }

                if (!this.Configuration.ArchiveCommand.Verify(this.file.File))
                {
                    this.Message = this.Configuration.ArchiveCommand.Message;
                    return false;
                }

                this.Message = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// Gets the view model for the SAP data of the archive action
        /// </summary>
        public SapDataViewModel Sap { get; }
    }
}