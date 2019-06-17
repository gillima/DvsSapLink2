using System.Windows;

namespace WpfDemo.Model
{
    public class Order
    {
        public int SapLabor;
        public string SapStatus;
        public string SapUser;

        public void DvsToSap()
        {
            MessageBox.Show(
                $"Labor: {this.SapLabor}, Status: {this.SapStatus}, User: {this.SapUser}", "DvsToSap",
                MessageBoxButton.OK);
        }
    }
}