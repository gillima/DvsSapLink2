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
        /// Validates the file content and compares the drawing number with the filename (including revision)
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private void Validate()
        {
            // validates whether the drawing filename drawingnumber matches the value in the attribute file
            var drawingAttribute = this.attributes.FirstOrDefault(a => a.Name == FileAttributeName.Zeichnungsnummer);
            if (drawingAttribute == null)
            {
                throw new FormatException(TXT_DRAWING_NUMBER_MISSING);
            }

            var drawingNumber = drawingAttribute.Value;
            // regex selects any character(s) left of the first "-"
            var match = Regex.Match(this.Title, "(.*?)-.*");
            if (!match.Success)
                throw new FormatException(TXT_INVALID_FILE_NAME);

            if (!string.Equals(match.Groups[1].Value, drawingNumber, StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException(TXT_DRAWING_NUMBER_MISMATCH);


            // validates whether the drawing filename revision matches the value in the attribute file
            var revisionAttribute = this.attributes.FirstOrDefault(a => a.Name == FileAttributeName.AeStand_aktuell);
            if (revisionAttribute == null)
            {
                throw new FormatException(TXT_REVISION_MISSING);
            }

            var revIndex = string.IsNullOrEmpty(revisionAttribute.Value)
                ? $"0"
                : revisionAttribute.Value.Replace("-", "0");
            // regex selects one or two character(s) between the first and second "-"
            match = Regex.Match(this.Title, ".*?-(.{1,2})-.*");
            if (!match.Success)
                throw new FormatException(TXT_INVALID_FILE_NAME);

            if (!string.Equals(match.Groups[1].Value, revIndex, StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException(TXT_REVISION_MISMATCH);


            // validates whether the drawing filename sheetnumber matches the value in the attribute file
            var sheetNoAttribute = this.attributes.FirstOrDefault(a => a.Name == FileAttributeName.BlattNr);
            if (sheetNoAttribute == null)
            {
                throw new FormatException(TXT_SHEET_NUMBER_MISSING);
            }

            var sheetNo = int.TryParse(sheetNoAttribute.Value.Trim(' ', '/'), out var number)
                ? $"{number,2:00}"
                : throw new FormatException(TXT_INVALID_SHEET_NUMBER);
            // regex selects any character(s) right of the second "-"
            match = Regex.Match(this.Title, ".*?-.{1,2}-(.*)");
            if (!match.Success)
                throw new FormatException(TXT_INVALID_FILE_NAME);

            if (!string.Equals(match.Groups[1].Value, sheetNo, StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException(TXT_SHEET_NUMBER_MISMATCH);

        }
    }
}
