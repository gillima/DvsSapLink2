using System;
using System.Collections.ObjectModel;
using System.Linq;
using DwgSapLink2.Model;
using GalaSoft.MvvmLight;

namespace DwgSapLink2.ViewModel
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
        }

        /// <summary>
        /// Gets the title of the file
        /// </summary>
        public string Title => this.attributeFile.Title;

        /// <summary>
        /// Gets a value indicating whether the file and the file content correspond to each other
        /// </summary>
        public bool IsValid
        {
            get
            {
                var drawingAttribute = this.attributes.FirstOrDefault(a => a.Name == FileAttributeName.ZeichnungsNummer);
                if (drawingAttribute == null) return false;

                return string.Equals(
                    this.attributeFile.Title,
                    drawingAttribute.Value,
                    StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Gets the file attributes of the file. The attributes are loaded lazy when the
        /// collection is accessed
        /// </summary>
        public ObservableCollection<FileAttribute> Attributes => this.attributes ?? (this.attributes = new ObservableCollection<FileAttribute>(this.attributeFile.Attributes));
    }
}