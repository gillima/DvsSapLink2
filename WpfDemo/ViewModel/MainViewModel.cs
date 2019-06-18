using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using WpfDemo.Model;
using GalaSoft.MvvmLight;

namespace WpfDemo.ViewModel
{
    public class FileSystemItem
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }

    public class DirectoryItem : FileSystemItem
    {
        public DirectoryItem(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileName(path);

            this.Items = new ObservableCollection<FileSystemItem>();
            try
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    this.Items.Add(new DirectoryItem(directory));
                }
                foreach (var file in Directory.GetFiles(path))
                {
                    this.Items.Add(new FileItem(file));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public ObservableCollection<FileSystemItem> Items { get; private set; }
    }

    public class FileItem : FileSystemItem
    {
        public FileItem(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileName(path);
        }
    }

    public class MainViewModel : ViewModelBase
    {
        private readonly Order order;
        private string currentFile;
        private string workingDirectory;
        private FileSystemItem directoryContent;

        public MainViewModel(Order order)
        {
            this.SelectFile = new BrowseFile(file => this.CurrentFile = file, () => this.CurrentFile);
            this.SelectWorkingDirectory= new BrowseDirectory(dir => this.WorkingDirectory = dir, () => this.WorkingDirectory);

            this.order = order;
            this.DirectoryContent = null;
            this.SapData = new SapData(order);
        }

        public ICommand SelectFile { get; }

        public ICommand SelectWorkingDirectory { get; }

        public string CurrentFile
        {
            get { return this.currentFile; }
            private set { this.Set(ref this.currentFile, value); }
        }

        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            private set
            {
                if (!this.Set(ref this.workingDirectory, value)) return;
                this.DirectoryContent = new DirectoryItem(this.workingDirectory);
            }
        }

        public SapData SapData { get; private set; }

        public FileSystemItem DirectoryContent
        {
            get { return this.directoryContent; }
            private set { this.Set(ref this.directoryContent, value); }
        }
    }
}