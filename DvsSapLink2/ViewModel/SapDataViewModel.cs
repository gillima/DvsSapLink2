using System.Collections.Generic;
using System.Configuration;
using DvsSapLink2.Model;
using DvsSapLink2.Settings;
using GalaSoft.MvvmLight;

namespace DvsSapLink2.ViewModel
{
    /// <summary>
    /// ViewModel providing the SAP data to the UI and adds meta information like list or labors and states
    /// </summary>
    public class SapDataViewModel : ViewModelBase
    {
        private readonly SapData sapData;

        /// <summary>
        /// Static constructor executed on application load. Loads the app.config settings to fill
        /// Labors and States to be used in combobox or other lists
        /// </summary>
        static SapDataViewModel()
        {
            var section = (GuiSettings)ConfigurationManager.GetSection("guiSettings");
            SapDataViewModel.Labors = section.Labors.As<string, int>();
            SapDataViewModel.States = section.States.As<string, string>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SapDataViewModel"/> class
        /// </summary>
        /// <param name="sapData">The Data Transfer Object (DTO) containing the SAP data</param>
        public SapDataViewModel(SapData sapData)
        {
            this.sapData = sapData;
        }

        /// <summary>
        /// Gets the list of labors for the selection box
        /// </summary>
        public static IDictionary<string, int> Labors { get; }

        /// <summary>
        /// Gets the list of document states for the selection box
        /// </summary>
        public static IDictionary<string, string> States { get; }

        /// <summary>
        /// Gets or sets the labor
        /// </summary>
        public int Labor
        {
            get => this.sapData.Labor;
            set
            {
                this.sapData.Labor = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string State
        {
            get => this.sapData.State;
            set
            {
                this.sapData.State = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SAP user
        /// </summary>
        public string User
        {
            get => this.sapData.User;
            set
            {
                this.sapData.User = value;
                this.RaisePropertyChanged();
            }
        }
    }
}