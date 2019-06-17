using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using WpfDemo.Model;

namespace WpfDemo.ViewModel
{
    public class SapData : ViewModelBase
    {
        private readonly Order order;

        public SapData(Order order)
        {
            this.order = order;

            this.Labors = new Dictionary<string, int>
            {
                { "Labor (100)", 100 },
                { "Labor (200)", 200 },
                { "Konstruktionsbüro (300)", 300 },
            };

            this.States = new Dictionary<string, string>
            {
                { "Preparation", "IN" },
                { "Production", "PR" },
                { "Delivery", "DE" },
                { "Done", "DO" },
            };

            this.CreateDvs = new RelayCommand(this.order.DvsToSap);
        }

        public IDictionary<string, int> Labors { get; }

        public IDictionary<string, string> States { get; }

        public ICommand CreateDvs { get; }

        public int Labor
        {
            get => this.order.SapLabor;
            set => this.Set(ref this.order.SapLabor, value);
        }


        public string State
        {
            get => this.order.SapStatus;
            set => this.Set(ref this.order.SapStatus, value);
        }

        public string User
        {
            get => this.order.SapUser;
            set => this.Set(ref this.order.SapUser, value);
        }
    }
}