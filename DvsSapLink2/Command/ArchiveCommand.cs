using System.IO;
using System.Linq;
using System.Windows;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using DvsSapLink2.ViewModel;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.Command
{
    public class ArchiveCommand : CopyCommand
    {
        private FileAttributeName[] attributesToLog =
        {
            FileAttributeName.ZeichnungsNummer,
            FileAttributeName.Massstab,
            FileAttributeName.Bemerkung,
        };
        
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

            var logFile = Path.Combine(this.configuration.LogDirectory, file.Title + ".log");
            var sapTransferFileTemp = Path.Combine(this.configuration.ArchiveSapTransferDirectory, file.Title + ".dat");
            var archiveDir = LogParser.ReadMessages(logFile, "A_DIR");
            MessageBox.Show($"Archive Directory: {archiveDir.FirstOrDefault()}");

            using (var logger = new Logger(logFile))
            {
                //foreach (var attribute in this.attributesToLog)
                //{
                //    logger.Write("INFO", $"{attribute}: {file[attribute]}");
                //}

                using (var sapTransferWriter = new SapTransferWriter(sapTransferFileTemp))
                {
                    sapTransferWriter.WriteFileAttributes(file, viewModel.Sap.Data);
                }

                //TODO: Text durch Textkonstante ersetzen
                logger.Write("LOG", "Transferdaten für SAP erstellt");

                //TODO: wieder aktivieren, da zum Testen vom log-File auskommentiert
                //this.CopyFile(file, ".dwg", this.configuration.DestinationDirectory, false);
                //this.CopyFile(file, ".dwg", this.configuration.ConversionDirectory, true);
                //this.CopyFile(file, ".txt", this.configuration.TxtDirectory, true);
                //this.DeleteFile(file, ".pdf");

                //TODO: Text durch Textkonstante ersetzen
                logger.Write("LOG", "Zeichnung archiviert");
            }

            MessageBox.Show(TXT_FILE_ARCHIVED, this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            
            // HACK: force update of file list
            viewModel.Configuration.SourceDirectory = viewModel.Configuration.SourceDirectory;
        }
    }
}