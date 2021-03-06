using System;
using System.IO;
using System.Windows;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using DvsSapLink2.Resources;
using DvsSapLink2.ViewModel;

namespace DvsSapLink2.Command
{
    public class PrepareCommand : CopyCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrepareCommand"/> class.
        /// </summary>
        public PrepareCommand(Configuration configuration)
            : base(configuration, Strings.TXT_DO_PREPARE)
        {
        }

        public override bool Verify(AttributeFile file = null)
        {
            if (!base.Verify(file)) return false;

            try
            {
                var fileToCheck = Path.Combine(this.configuration.PendingDirectory, file.Title + ".dwg");
                if (File.Exists(fileToCheck))
                    throw new InvalidOperationException(Strings.TXT_PENDING_FILE_EXISTS);

                return true;
            }
            catch (InvalidOperationException ex)
            {
                this.Message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Executes the prepare archive command
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        public override void Execute(object parameter)
        {
            var viewModel = (MainViewModel)parameter;
            var file = viewModel.File.File;

            using (var logger = new Logger(Path.Combine(this.configuration.LogDirectory, file.Title + ".log"),false))
            {
                logger.Write("W_DIR", this.configuration.SourceDirectory);
                logger.Write("A_DIR", this.configuration.DestinationDirectory);
                logger.Write("STATE", viewModel.Sap.Data.State);
                logger.Write("ATEX", viewModel.Sap.Data.Atex);
                logger.Write("ORDER", viewModel.Sap.Data.OrderState);
                logger.Write("CLASS", viewModel.Sap.Data.Classification);
                logger.Write("PROJ", viewModel.Sap.Data.Project);
                logger.Write("CONT", viewModel.Sap.Data.DocContent);

                this.CopyFile(file, ".dwg", this.configuration.PendingDirectory);
                this.CopyFile(file, ".pdf", this.configuration.PendingDirectory);
                this.CopyFile(file, ".txt", this.configuration.PendingDirectory);

                this.DeleteFile(file, ".pdf");
                this.DeleteFile(file, ".dwg");
                this.DeleteFile(file, ".txt");
                this.DeleteFile(file, ".bak");
            }

            MessageBox.Show(Strings.TXT_FILE_ARCHIVED, this.Title, MessageBoxButton.OK, MessageBoxImage.Information);

            // HACK: force update of file list
            viewModel.Configuration.SourceDirectory = viewModel.Configuration.SourceDirectory;
        }
    }
}
