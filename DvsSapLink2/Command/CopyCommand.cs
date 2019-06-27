using System;
using System.IO;
using System.Windows.Input;
using DvsSapLink2.Model;
using DvsSapLink2.Resources;
using DvsSapLink2.ViewModel;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.Command
{
    public abstract class CopyCommand : ICommand
    {
        protected readonly Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyCommand"/> class.
        /// </summary>
        protected CopyCommand(Configuration configuration, string title)
        {
            this.configuration = configuration;
            this.Title = title;
        }

        /// <summary>
        /// Event raised to inform that the execute status of the command has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
       
        /// <summary>
        /// Gets the title of the command
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets a message indicating why the command cannot be executed
        /// </summary>
        public string Message { get; private set; }
        
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
        public virtual bool Verify(AttributeFile file)
        {
            try
            {
                if (file == null || !File.Exists(file.Path))
                    throw new InvalidOperationException(Strings.TXT_NO_FILE_SELECTED);
                
                if (!Directory.Exists(this.configuration.DestinationDirectory))
                    throw new InvalidOperationException(Strings.TXT_DESTINATION_MISSING);
                
                var fileToCheck = Path.Combine(this.configuration.DestinationDirectory, file.Title + ".dwg");
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
        /// Copy the file with the given extension related to the attribute file into the
        /// destination directory
        /// </summary>
        /// <param name="file">The attribute file</param>
        /// <param name="fileExtension">File extension of the file to copy</param>
        /// <param name="destinationDirectory">Destination directory</param>
        protected void CopyFile(AttributeFile file, string fileExtension, string destinationDirectory)
        {
            if (file?.Path == null)
                throw new InvalidOperationException(Strings.TXT_NO_FILE_SELECTED);
            
            var source = Path.Combine(
                Path.GetDirectoryName(file.Path),
                file.Title + fileExtension);
                
            if (!File.Exists(source))
                throw new InvalidOperationException(Strings.TXT_SOURCE_FILE_MISSING);
             
            var destination = Path.Combine(
                destinationDirectory,
                file.Title + fileExtension);
            
            File.Copy(source, destination);
            if (!File.Exists(destination))
                throw new IOException(TXT_COPY_FAILED);
                
            File.Delete(source);
        }
        
        /// <summary>
        /// Delete the file with the given extension related to the attribute file
        /// </summary>
        /// <param name="file">The attribute file</param>
        /// <param name="fileExtension">File extension of the file to delete</param>
        protected void DeleteFile(AttributeFile file, string fileExtension)
        {
            if (file?.Path == null || !File.Exists(file.Path))
                throw new InvalidOperationException(Strings.TXT_NO_FILE_SELECTED);
            
            var source = Path.Combine(
                Path.GetDirectoryName(file.Path),
                file.Title + fileExtension);
            
            if (File.Exists(source))
                File.Delete(source);
        }
    }
}