using System;
using System.Windows.Input;
using Microsoft.Win32;

namespace WpfDemo.ViewModel
{
    public class BrowseDirectory : ICommand
    {
        private readonly Func<string> currentDirectory;
        private readonly Action<string> selectDirectory;

        public BrowseDirectory(Action<string> selectDirectory, Func<string> currentDirectory = null)
        {
            this.currentDirectory = currentDirectory;
            this.selectDirectory = selectDirectory;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var browseFileDialog = new OpenFileDialog
            {
                InitialDirectory= this.currentDirectory() ?? string.Empty,
                Title = "Select a Directory",
                Filter = "Directory|*.this.directory",
                FileName = "select",
                CheckFileExists = false
            };

            if (browseFileDialog.ShowDialog() != true)
                return;

            var selectedPath = browseFileDialog.FileName
                .Replace("\\select.this.directory", "")
                .Replace(".this.directory", "");

            this.selectDirectory(selectedPath);
        }
    }
}
