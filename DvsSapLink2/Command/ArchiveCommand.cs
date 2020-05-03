using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using DvsSapLink2.Resources;
using DvsSapLink2.ViewModel;

namespace DvsSapLink2.Command
{
    public class ArchiveCommand : CopyCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCommand"/> class.
        /// </summary>
        public ArchiveCommand(Configuration configuration)
            : base(configuration, Strings.TXT_DO_ARCHIVE)
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

            // var timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            var logFile = Path.Combine(this.configuration.LogDirectory, file.Title + ".log");
            var sapTransferFileTemp = Path.Combine(this.configuration.SourceDirectory, file.Title + ".dat");
            var archiveDir = LogParser.ReadMessages(logFile, "A_DIR");
            this.configuration.DestinationDirectory = archiveDir.FirstOrDefault();
            // MessageBox.Show($"Archive Directory: {archiveDir.FirstOrDefault()}");
            var archiveUser = LogParser.ReadMessages(logFile, "USER");
            viewModel.Sap.Data.User = archiveUser.FirstOrDefault();
            var docState = LogParser.ReadMessages(logFile, "STATE");
            viewModel.Sap.Data.State = docState.FirstOrDefault();
            var atex = LogParser.ReadMessages(logFile, "ATEX");
            viewModel.Sap.Data.Atex = docState.FirstOrDefault();
            var orderState = LogParser.ReadMessages(logFile, "ORDER");
            viewModel.Sap.Data.OrderState = orderState.FirstOrDefault();
            var classification = LogParser.ReadMessages(logFile, "CLASS");
            viewModel.Sap.Data.Classification = classification.FirstOrDefault();
            var docContent = LogParser.ReadMessages(logFile, "CONT");
            viewModel.Sap.Data.DocContent = docContent.FirstOrDefault();
            var project = LogParser.ReadMessages(logFile, "PROJ");
            viewModel.Sap.Data.Project = docContent.FirstOrDefault();

            // TODO: Logger erzeugt einen Konflikt, weil LogParser auf gleiches File zugreift. Zum Testen auskommentiert.
            //using (var logger = new Logger(logFile, true))
            {
                this.WriteEloFile(sapTransferFileTemp, file, viewModel);
            //    logger.Write("LOG", Strings.TXT_TRANSFERFILE_CREATED);

                this.CopyFile(file, ".dwg", this.configuration.DestinationDirectory);
                this.CopyFile(file, ".dwg", this.configuration.ConversionDirectory);
                this.CopyFile(file, ".txt", this.configuration.TxtDirectory);
                this.CopyFile(file, ".dat", this.configuration.SapTransferDirectory, ".txt");
                this.CopyFile(file, ".dat", this.configuration.ArchiveSapTransferDirectory);

                this.DeleteFile(file, ".pdf");
                this.DeleteFile(file, ".dat");
                this.DeleteFile(file, ".txt");
                this.DeleteFile(file, ".dwg");

            //    logger.Write("LOG", Strings.TXT_DRAWINGFILE_ARCHIVED);
            }

            MessageBox.Show(Strings.TXT_FILE_ARCHIVED, this.Title, MessageBoxButton.OK, MessageBoxImage.Information);

            // HACK: force update of file list
            viewModel.Configuration.SourceDirectory = viewModel.Configuration.SourceDirectory;
        }

        private void WriteEloFile(string fileName, AttributeFile file, MainViewModel viewModel)
        {
            using (var stream = new StreamWriter(fileName, false, System.Text.Encoding.GetEncoding("ISO-8859-1")))
            {
                var attributes = this.GetFileAttributes(file, viewModel.Sap.Data);
                foreach (var attribute in attributes
                    .Where(a => a.Name.GetOrder() != 0)
                    .OrderBy(a => a.Name.GetOrder()))
                {
                    this.WriteEloAttribute(stream, attribute.Name, attribute.Value);
                }
            }
        }

        /// <summary>
        /// Updates the attributes of the file by the data of the SAP view model
        /// </summary>
        /// <param name="file">The attribute file</param>
        /// <param name="sapData">SAP view model</param>
        private IEnumerable<FileAttribute> GetFileAttributes(AttributeFile file, SapData sapData)
        {
            foreach (var attribute in file.Attributes)
            {
                yield return attribute;
            }

            foreach (var converter in FileAttributeParser.ConvertDefinitions)
            {
                yield return new FileAttribute(
                    converter.Key,
                    null,
                    converter.Value(file, sapData));
            }
        }

        /// <summary>
        /// Format and write a ELO attribute line
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void WriteEloAttribute(StreamWriter stream, FileAttributeName name, string value)
        {
            var line = string.IsNullOrEmpty(value)
                ? $"{name.GetDescription()}"
                : $"{name.GetDescription(),-25} = {value}";
            stream.WriteLine(line);
        }
    }
}
