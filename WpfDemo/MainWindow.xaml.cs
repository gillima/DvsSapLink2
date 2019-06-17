using System.Windows;
using Irene.ViewModel;
using WpfDemo.Model;

namespace WpfDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var order = new Order
            {
                SapLabor = 200,
                SapStatus = "DO",
                SapUser = "123456"
            };
            this.ViewModel = new MainViewModel(order);

            this.InitializeComponent();
        }

        public MainViewModel ViewModel { get; }
    }
}
