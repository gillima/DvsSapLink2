using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using DvsSapLink2.Model;
using DvsSapLink2.Resources;
using DvsSapLink2.ViewModel;

namespace DvsSapLink2.Command
{
    public abstract class CopyCommand : ICommand
    {
        protected readonly Configuration configuration;
        private readonly Timer validationTimer;
        private AttributeFile lastAttributeFile;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyCommand"/> class.
        /// </summary>
        protected CopyCommand(Configuration configuration, string title)
        {
            this.configuration = configuration;
            this.Title = title;
            this.validationTimer = new Timer(2000)
            {
                AutoReset = true,
                Enabled = true,
            };
            this.validationTimer.Elapsed += this.ReValidate;
        }

        /// <summary>
        /// Event raised to inform that the execute status of the command has changed
        /// </summary>
        public event EventHandler CanExecuteChanged;
       
        /// <summary>
        /// Gets the title of the command
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets a message indicating why the command cannot be executed
        /// </summary>
        public string Message { get; protected set; }
        
        /// <summary>
        /// Returns a value indicating whether the command can be executed or not
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        /// <returns><c>true</c> if the command can be executed; <c>false</c> otherwise</returns>
        public bool CanExecute(object parameter)
        {
            if (!(parameter is MainViewModel)) return false;
            var viewModel = (MainViewModel) parameter;
            return viewModel.IsValid && this.Verify(viewModel.File.File);
        }
        
        /// <summary>
        /// Executes the prepare archive command
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Verify that the file doesn't exists at the destination and the destination is writable
        /// </summary>
        /// <param name="file">Source file to copy</param>
 
        public virtual bool Verify(AttributeFile file = null)
        {
            if (file == null) file = this.lastAttributeFile;
            this.lastAttributeFile = file;
           
            try
            {
                if (file == null || !File.Exists(file.Path))
                    throw new InvalidOperationException(Strings.TXT_NO_FILE_SELECTED);

                if (file.Title.ToString().Length > 15)
                    throw new InvalidOperationException(Strings.TXT_INVALID_FILE_NAME);

                this.ValidateDate(file, FileAttributeName.ErstelltDatum, true);
                this.ValidateDate(file, FileAttributeName.GeprueftDatum, true);
                this.ValidateDate(file, FileAttributeName.FreigegebenDatum, true);
                this.ValidateDate(file, FileAttributeName.AuftragsNummer, false);


                var fileToCheck = Path.Combine(this.configuration.SourceDirectory, file.Title + ".pdf");
                if (!File.Exists(fileToCheck))
                    throw new InvalidOperationException(Strings.TXT_PDF_DOES_NOT_EXIST);

                // funktioniert zwar, Anzeige wird aber nicht akutalisiert wenn das File geschlossen wird. Deshalb vorl�ufig deaktiviert.
                if (this.IsFileLocked(fileToCheck))
                    throw new InvalidOperationException(Strings.TXT_PDF_FILE_IS_OPENED);

                if (!Directory.Exists(this.configuration.DestinationDirectory))
                    throw new InvalidOperationException(Strings.TXT_DESTINATION_MISSING);
                
                fileToCheck = Path.Combine(this.configuration.ArchiveTifDirectory, file.Title + ".tif");
                if (File.Exists(fileToCheck))
                    throw new InvalidOperationException(Strings.TXT_DESTINATION_FILE_EXISTS);

                return true;
            }
            catch (InvalidOperationException ex)
            {
                this.Message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Validate a date. required format is yyyy-mm-dd
        /// </summary>
        /// <param name="file">Attribute file</param>
        /// <param name="name">name of the attribute to check</param>
        /// <param name="required">Indicates if a date is required or not</param>
        private void ValidateDate(AttributeFile file, FileAttributeName name, bool required)
        {
            if (!required && string.IsNullOrEmpty(file[name]))
                return;

            if (string.IsNullOrEmpty(file[name]))
                throw new InvalidOperationException($"{Strings.TXT_MISSING_DATE}: {name}");

            if (!Regex.IsMatch(file[name], "\\d{4}-\\d{2}-\\d{2}"))
                throw new InvalidOperationException($"{Strings.TXT_INVALID_DATE}: {name}");
        }

        /// <summary>
        /// Validate a ordernumber. required format is 
        /// - RFQ_123456789
        /// - nicht 9 Zeichen lang
        /// - nicht 1- oder 1-...
        /// </summary>
        /// <param name="file">Attribute file</param>
        /// <param name="name">name of the attribute to check</param>
        /// <param name="required">Indicates if a date is required or not</param>
        private void ValidateOrderNumber(AttributeFile file, FileAttributeName name, bool required)
        {
            if (!required && string.IsNullOrEmpty(file[name]))
                return;

            if (string.IsNullOrEmpty(file[name]))
                throw new InvalidOperationException($"{Strings.TXT_MISSING_DATE}: {name}");

            if (Regex.IsMatch(file[name], "(^RFQ_.{0,8}$)|(^RFQ_.{10,99}$)|^1-$|^1-[.]+|^[0-9]{9}$"))
                throw new InvalidOperationException($"{Strings.TXT_INVALID_ORDER_NUMBER}: {name}");
        }


        /// <summary>
        /// Copy the file with the given extension related to the attribute file into the
        /// destination directory
        /// </summary>
        /// <param name="file">The attribute file</param>
        /// <param name="fileExtension">File extension of the file to copy</param>
        /// <param name="destinationDirectory">Destination directory</param>
        protected void CopyFile(AttributeFile file, string fileExtensionSource, string destinationDirectory, string fileExtensionDestination = "")
        {
            if (file?.Path == null)
                throw new InvalidOperationException(Strings.TXT_NO_FILE_SELECTED);
            
            var source = Path.Combine(
                Path.GetDirectoryName(file.Path),
                file.Title + fileExtensionSource);
                
            if (!File.Exists(source))
                throw new InvalidOperationException(Strings.TXT_SOURCE_FILE_MISSING);

            if (fileExtensionDestination == "")
                fileExtensionDestination = fileExtensionSource;

            var destination = Path.Combine(
                destinationDirectory,
                file.Title + fileExtensionDestination);
            
            File.Copy(source, destination);
            if (!File.Exists(destination))
                throw new IOException(Strings.TXT_COPY_FAILED);
        
        }
        
        /// <summary>
        /// Delete the file with the given extension related to the attribute file
        /// </summary>
        /// <param name="file">The attribute file</param>
        /// <param name="fileExtension">File extension of the file to delete</param>
        protected void DeleteFile(AttributeFile file, string fileExtension)
        {
            if (file?.Path == null)
                throw new InvalidOperationException(Strings.TXT_NO_FILE_SELECTED);

            var source = Path.Combine(
                Path.GetDirectoryName(file.Path),
                file.Title + fileExtension);

            if (File.Exists(source))
                File.Delete(source);
        }

        /// <summary>
        /// Checks if the file is locked by another application
        /// </summary>
        /// <param name="fileToCheck">Path to the file to check</param>
        /// <returns><c>true</c> if the file is locked by another application, <c>false</c> otherwise</returns>
        private bool IsFileLocked(string fileToCheck)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(fileToCheck, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }

        /// <summary>
        /// Background task to re-validate the file
        /// </summary>
        private void ReValidate(object source, ElapsedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}