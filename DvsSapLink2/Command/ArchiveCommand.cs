using System.IO;
using System.Windows;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using DvsSapLink2.ViewModel;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.Command
{
    public class ArchiveCommand : CopyCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCommand"/> class.
        /// </summary>
        public ArchiveCommand(Configuration configuration)
            : base(configuration, TXT_DO_ARCHIVE)
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
                
                this.CopyFile(file, ".dwg", this.configuration.DestinationDirectory, false);
                this.CopyFile(file, ".dwg", this.configuration.ConversionDirectory, true);
                this.CopyFile(file, ".txt", this.configuration.TxtDirectory, true);
                this.DeleteFile(file, ".pdf");

                //TODO: Text durch Textkonstante ersetzen
                logger.Write("LOG", "Zeichnung archiviert");

                //TODO: SapTransfer-Datei erstellen nach SAPTransfer-Directory
                //TODO: SapTransfer-Datei erstellen nach SAPTransferArchiv-Directory

                //TODO: Text durch Textkonstante ersetzen
                logger.Write("LOG", "Transferdaten für SAP erstellt und archiviert");
            }

            MessageBox.Show(TXT_FILE_ARCHIVED, this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            
            // HACK: force update of file list
            viewModel.Configuration.SourceDirectory = viewModel.Configuration.SourceDirectory;
        }
    }
}