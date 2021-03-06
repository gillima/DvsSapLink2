﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DvsSapLink2.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value is bool boolArgument && boolArgument;
            if (parameter?.ToString() == "Invert") boolValue = !boolValue;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}