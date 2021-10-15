using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GameModManager.Services.DataConverter
{
    /// <summary>
    /// Converter class to negate boolean value
    /// </summary>
    class NegateBoolConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }
    }
}
