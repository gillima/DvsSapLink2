using System.IO;
using System.Windows;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using DvsSapLink2.ViewModel;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.Command
{
    public class PrepareCommand : CopyCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrepareCommand"/> class.
        /// </summary>
        public PrepareCommand(Configuration configuration)
            : base(configuration, TXT_DO_PREPARE)
        {
        }

        /// <summary>
        /// Executes the prepare archive command
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        public override void Execute(object parameter)
        {
            var viewModel = (MainViewModel)parameter;
            var file = viewModel.File.File;

            using (var logger = new Logger(Path.Combine(this.configuration.LogDirectory, file.Title + ".log")))
            {
                logger.Write("W_DIR", this.configuration.SourceDirectory);
                logger.Write("A_DIR", this.configuration.DestinationDirectory);

                this.CopyFile(file, ".dwg", this.configuration.DestinationDirectory);
                this.CopyFile(file, ".pdf", this.configuration.DestinationDirectory);
                this.CopyFile(file, ".txt", this.configuration.DestinationDirectory);
                // this.DeleteFile(file, ".bak");
            }

            MessageBox.Show(TXT_FILE_ARCHIVED, this.Title, MessageBoxButton.OK, MessageBoxImage.Information);

            // HACK: force update of file list
            viewModel.Configuration.SourceDirectory = viewModel.Configuration.SourceDirectory;
        }
    }
}