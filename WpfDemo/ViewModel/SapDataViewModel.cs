using System.Collections.Generic;
using DwgSapLink2.Model;
using GalaSoft.MvvmLight;

namespace DwgSapLink2.ViewModel
{
    /// <summary>
    /// ViewModel providing the SAP data to the UI and adds meta information like list or labors and states
    /// </summary>
    public class SapDataViewModel : ViewModelBase
    {
        private readonly SapData sapData;

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
        public IDictionary<string, int> Labors { get; } = new Dictionary<string, int>
            {
                { "Labor (100)", 100 },
                { "Labor (200)", 200 },
                { "Konstruktionsbüro (300)", 300 },
            };

        /// <summary>
        /// Gets the list of document states for the selection box
        /// </summary>
        public IDictionary<string, string> States { get; } = new Dictionary<string, string>
            {
                { "Preparation", "IN" },
                { "Production", "PR" },
                { "Delivery", "DE" },
                { "Done", "DO" },
            };

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