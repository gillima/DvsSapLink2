using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace DvsSapLink2.Helper
{
    public class BrowseDirectoryCommand : ICommand
    {
        private readonly Func<string> currentDirectory;
        private readonly Action<string> selectDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowseDirectoryCommand"/> class
        /// </summary>
        /// <param name="selectDirectory"></param>
        /// <param name="currentDirectory"></param>
        public BrowseDirectoryCommand(Action<string> selectDirectory, Func<string> currentDirectory = null)
        {
            this.currentDirectory = currentDirectory;
            this.selectDirectory = selectDirectory;
        }

        /// <summary>
        /// Raised when the executable state of the command has changed
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets whether the command can be executed or not
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; <c>false</c> otherwise</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Shows a directory browse dialog. The dialog initially used the directory provided by
        /// the <c>currentDirectory</c> callback. Once a directory is selected, the directory is
        /// passed by using the <c>selectDirectory</c> callback.
        /// </summary>
        public void Execute(object parameter)
        {
            var browseFolderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = this.currentDirectory() ?? string.Empty,
                ShowNewFolderButton = false,
            };

            if (browseFolderDialog.ShowDialog() != DialogResult.OK)
                return;

            this.selectDirectory(browseFolderDialog.SelectedPath);
        }
    }
}
