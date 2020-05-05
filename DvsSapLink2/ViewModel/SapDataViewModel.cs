using System.Collections.Generic;
using System.Configuration;
using DvsSapLink2.Model;
using DvsSapLink2.Settings;
using GalaSoft.MvvmLight;

namespace DvsSapLink2.ViewModel
{
    /// <summary>
    /// ViewModel providing the SAP data to the UI and adds meta information like lists
    /// </summary>
    public class SapDataViewModel : ViewModelBase
    {
        private readonly SapData sapData;

        /// <summary>
        /// Static constructor executed on application load. Loads the app.config settings to fill
        /// Values to be used in combobox or other lists
        /// </summary>
        static SapDataViewModel()
        {
            var section = (GuiSettings)ConfigurationManager.GetSection("guiSettings");
            SapDataViewModel.States = section.States.As<string, string>();
            SapDataViewModel.Atexs = section.Atexs.As<string, string>();
            SapDataViewModel.OrderStates = section.OrderStates.As<string, string>();
            SapDataViewModel.Classifications = section.Classifications.As<string, string>();
            SapDataViewModel.Projects = section.Projects.As<string, string>();
            SapDataViewModel.DocContents = section.DocContents.As<string, string>();
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
        /// Gets the SAP data model object
        /// </summary>
        public SapData Data => this.sapData;

        /// <summary>
        /// Gets the list of document states for the selection box
        /// </summary>
        public static IDictionary<string, string> States { get; }

        /// <summary>
        /// Gets the list of Atex states for the selection box
        /// </summary>
        public static IDictionary<string, string> Atexs { get; }

        /// <summary>
        /// Gets the list of document order states for the selection box
        /// </summary>
        public static IDictionary<string, string> OrderStates { get; }

        /// <summary>
        /// Gets the list of document classifications for the selection box
        /// </summary>
        public static IDictionary<string, string> Classifications { get; }

        /// <summary>
        /// Gets the list of document contents for the selection box
        /// </summary>
        public static IDictionary<string, string> DocContents { get; }

        /// <summary>
        /// Gets the list of document contents for the selection box
        /// </summary>
        public static IDictionary<string, string> Projects { get; }

        /// <summary>
        /// Gets the list of users for the selection box
        /// </summary>
        public IDictionary<string, string> Users => this.sapData.Users;


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
        /// Gets or sets the ATEX state
        /// </summary>
        public string Atex
        {
            get => this.sapData.Atex;
            set
            {
                this.sapData.Atex = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the document OrderState
        /// </summary>
        public string OrderState
        {
            get => this.sapData.OrderState;
            set
            {
                this.sapData.OrderState = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the document classification
        /// </summary>
        public string Classification
        {
            get => this.sapData.Classification;
            set
            {
                this.sapData.Classification = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the document state
        /// </summary>
        public string DocContent
        {
            get => this.sapData.DocContent;
            set
            {
                this.sapData.DocContent = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the project name
        /// </summary>
        public string Project
        {
            get => this.sapData.Project;
            set
            {
                this.sapData.Project = value;
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