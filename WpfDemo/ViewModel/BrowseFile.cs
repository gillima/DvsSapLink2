using System;
using System.Windows.Input;
using Microsoft.Win32;

namespace WpfDemo.ViewModel
{
    public class BrowseFile : ICommand
    {
        private readonly Func<string> currentFile;
        private readonly Action<string> selectFile;

        public BrowseFile(Action<string> selectFile, Func<string> currentFile = null)
        {
            this.currentFile = currentFile;
            this.selectFile = selectFile;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var browseFileDialog = new OpenFileDialog { FileName = this.currentFile() ?? string.Empty };
            if (browseFileDialog.ShowDialog() == true)
            {
                this.selectFile(browseFileDialog.FileName);
            }
        }
    }
}
