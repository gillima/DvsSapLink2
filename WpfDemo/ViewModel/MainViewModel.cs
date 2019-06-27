using System.Collections.ObjectModel;
using DwgSapLink2.Model;
using GalaSoft.MvvmLight;

namespace DwgSapLink2.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string message;
        private AttributeFileViewModel file;

        /// <summary>
        /// Creates a new instance of the <see cref="MainViewModel"/> class
        /// </summary>
        /// <param name="configuration">Configuration to use</param>
        public MainViewModel(Configuration configuration)
        {
            this.Configuration = new ConfigurationViewModel(configuration, this.Files);
            this.Sap = new SapDataViewModel(new SapData());
            this.Message = "hello world";
            this.File = null;
        }

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
                if (value == null) this.Message = "No file selected";
            }
        }

        /// <summary>
        /// Gets the view model for the SAP data of the archive action
        /// </summary>
        public SapDataViewModel Sap { get; }
    }
}