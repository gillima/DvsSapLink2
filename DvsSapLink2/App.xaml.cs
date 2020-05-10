using System.Linq;
using System.Windows;
using DvsSapLink2.Model;
using DvsSapLink2.ViewModel;

namespace DvsSapLink2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs startup)
        {
            base.OnStartup(startup);

            // parse command line arguments and look for "archive
            var configurationType = startup.Args.FirstOrDefault(a => a.ToLowerInvariant().Contains("archive")) != null
                ? ConfigurationType.Archive
                : ConfigurationType.Prepare;

            var configuration = new Configuration(configurationType);
            var sapData = new SapData(configurationType);
            
            if (configurationType == ConfigurationType.Prepare)
            {
                // create the main window and set data context
                this.MainWindow = new PrepareView
                {
                    DataContext = new MainViewModel(configuration, sapData)
                };
            }
            else
            {
                // create the main window and set data context
                this.MainWindow = new ArchiveView
                {
                    DataContext = new MainViewModel(configuration, sapData)
                };
            }

            // show the main window
            this.MainWindow.Show();
        }
    }
}
