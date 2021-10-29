using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GameModManager.Services.DataConverter
{
    /// <summary>
    /// Converter class to convert version to valid string
    /// </summary>
    class VersionToString : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Version version)
            {
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            return string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string version)
            {
                return new Version(string.Format("{0}.{1}", version, 0));
            }
            return new Version();
        }
    }
}
