using System;
using System.Globalization;
using System.Windows.Data;
using DvsSapLink2.Model;

namespace DvsSapLink2.Converter
{
    public class TypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value is ConfigurationType configValue && configValue == ConfigurationType.Prepare;
            return parameter?.ToString() == "inverted"
                ? !result
                : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}