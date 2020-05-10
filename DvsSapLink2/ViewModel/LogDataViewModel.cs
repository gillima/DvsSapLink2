using System.IO;
using System.Linq;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using GalaSoft.MvvmLight;

namespace DvsSapLink2.ViewModel
{
    public class LogDataViewModel : ViewModelBase
    {
        private readonly Configuration configuration;
        private string file;
        private string sourceDirectory;
        private string docContent;
        private string project;
        private string classification;
        private string orderState;
        private string state;
        private string atex;
        private string user;
        private string destinationDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogDataViewModel"/> class.
        /// </summary>
        public LogDataViewModel(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public string File
        {
            get => this.file;
            set
            {
                this.file = value;
                this.ParseLogFile();
            }
        }

        public string SourceDirectory
        {
            get => this.sourceDirectory;
            private set => this.Set(ref this.sourceDirectory, value);
        }

        public string DestinationDirectory
        {
            get => this.destinationDirectory;
            private set => this.Set(ref this.destinationDirectory, value);
        }

        public string User
        {
            get => this.user;
            private set => this.Set(ref this.user, value);
        }

        public string State
        {
            get => this.state;
            private set => this.Set(ref this.state, value);
        }

        public string Atex
        {
            get => this.atex;
            private set => this.Set(ref this.atex, value);
        }

        public string OrderState
        {
            get => this.orderState;
            private set => this.Set(ref this.orderState, value);
        }

        public string Classification
        {
            get => this.classification;
            private set => this.Set(ref this.classification, value);
        }

        public string Project
        {
            get => this.project;
            private set => this.Set(ref this.project, value);
        }

        public string DocContent
        {
            get => this.docContent;
            private set => this.Set(ref this.docContent, value);
        }

        private void ParseLogFile()
        {
            var logFile = Path.Combine(
                this.configuration.LogDirectory,
                this.file + ".log");
            
            if (!System.IO.File.Exists(logFile)) return;

            this.SourceDirectory = LogParser.ReadMessages(logFile, "W_DIR").FirstOrDefault();
            this.DestinationDirectory = LogParser.ReadMessages(logFile, "A_DIR").FirstOrDefault();
            this.User = LogParser.ReadMessages(logFile, "USER").FirstOrDefault();
            this.State = LogParser.ReadMessages(logFile, "STATE").FirstOrDefault();
            this.Atex = LogParser.ReadMessages(logFile, "ATEX").FirstOrDefault();
            this.OrderState = LogParser.ReadMessages(logFile, "ORDER").FirstOrDefault();
            this.Classification = LogParser.ReadMessages(logFile, "CLASS").FirstOrDefault();
            this.Project = LogParser.ReadMessages(logFile, "PROJ").FirstOrDefault();
            this.DocContent = LogParser.ReadMessages(logFile, "CONT").FirstOrDefault();
        }
    }
}