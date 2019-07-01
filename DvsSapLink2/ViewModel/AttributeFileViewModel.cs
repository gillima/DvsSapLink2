using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DvsSapLink2.Command;
using DvsSapLink2.Model;
using GalaSoft.MvvmLight;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.ViewModel
{
    public class AttributeFileViewModel : ViewModelBase
    {
        private readonly AttributeFile attributeFile;
        private ObservableCollection<FileAttribute> attributes;

        /// <summary>
        /// Creates a new instance of the <see cref="AttributeFileViewModel"/> class
        /// </summary>
        /// <param name="attributeFile">DTO of the file represented by this view model</param>
        public AttributeFileViewModel(AttributeFile attributeFile)
        {
            this.attributeFile = attributeFile;
            this.OpenFile = new OpenFileCommand(this.attributeFile);
        }

        /// <summary>
        /// Gets the title of the file
        /// </summary>
        public string Title => this.attributeFile.Title;

        /// <summary>
        /// Gets the attribute file represented by this view model
        /// </summary>
        public AttributeFile File => this.attributeFile;

        /// <summary>
        /// Command to open a file using windows default
        /// </summary>
        public ICommand OpenFile { get; }

        /// <summary>
        /// Gets the validation message for this file
        /// </summary>
        public string Message { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether the file and the file content correspond to each other
        /// </summary>
        public bool IsValid
        {
            get
            {
                try
                {
                    this.Validate();
                    this.Message = string.Empty;
                    return true;
                }
                catch (FormatException ex)
                {
                    this.Message = ex.Message;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the file attributes of the file. The attributes are loaded lazy when the
        /// collection is accessed
        /// </summary>
        public ObservableCollection<FileAttribute> Attributes => this.attributes ?? (this.attributes = new ObservableCollection<FileAttribute>(this.attributeFile.Attributes));

        /// <summary>
        /// Validates the file content and compares the drawing number with the filename
        /// </summary>
        /// <exception cref="FormatException"></exception>
        public void Validate()
        {
            var drawingAttribute = this.attributes.FirstOrDefault(a => a.Name == FileAttributeName.ZeichnungsNummer);
            if (drawingAttribute == null)
            {
                throw new FormatException(TXT_DRAWING_NUMBER_MISSING);
            }

            var drawingNumber = drawingAttribute.Value;
            var match = Regex.Match(this.Title, "(.*?)-.*");
            if (!match.Success)
                throw new FormatException(TXT_INVALID_FILE_NAME);

            if (!string.Equals(match.Groups[1].Value, drawingNumber, StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException(TXT_DRAWING_NUMBER_MISMATCH);
        }
    }
}