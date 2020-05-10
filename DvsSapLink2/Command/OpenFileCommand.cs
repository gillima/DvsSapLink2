using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DvsSapLink2.Model;

namespace DvsSapLink2.Command
{
    public class OpenFileCommand : ICommand
    {
        /// <summary>
        /// list of supported file extensions (lowercase without ".")
        /// </summary>
        private static readonly string[] SupportedExtensions = { "pdf", "docx", "txt", "log" };

        private readonly AttributeFile attributeFile;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenFileCommand"/> class.
        /// </summary>
        /// <param name="attributeFile">Attribute file to open related files for</param>
        public OpenFileCommand(AttributeFile attributeFile)
        {
            this.attributeFile = attributeFile;
        }

        /// <summary>
        /// Notifies if the execute state of the command has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Gets a value indicating whether the related file can be opened or not
        /// </summary>
        /// <param name="parameter">File extension of the related file to open</param>
        public bool CanExecute(object parameter)
        {
            if (!OpenFileCommand.SupportedExtensions.Contains($"{parameter}".ToLowerInvariant()))
                return false;

            var path = this.GetRelatedFileByExtension($"{parameter}");
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        /// <summary>
        /// Executes the command and opens the file using the default windows handler for the file extension
        /// </summary>
        /// <param name="parameter">File extension of the related file to open</param>
        public void Execute(object parameter)
        {
            if (!this.CanExecute(parameter))
                return;

            Process.Start(
                this.GetRelatedFileByExtension($"{parameter}"));
        }

        /// <summary>
        /// Combines the original file path and title with the file extension of the file to open
        /// </summary>
        /// <param name="extension">File extension of the related file to open</param>
        /// <returns>Full qualified file path to the related file to open</returns>
        private string GetRelatedFileByExtension(string extension)
        {
            return Path.Combine(
                    Path.GetDirectoryName(this.attributeFile.Path) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(this.attributeFile.Path)
                    + $".{extension}");
        }
    }
}