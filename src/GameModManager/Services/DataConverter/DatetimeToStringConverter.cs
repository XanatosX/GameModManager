using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace GameModManager.Services.DataConverter
{
    /// <summary>
    /// Class to convert Datetime to special formatted string
    /// </summary>
    class DatetimeToStringConverter : IValueConverter
    {
        private const string DEFAULT_RETURN = "Never";

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime == new DateTime())
                {
                    return DEFAULT_RETURN;
                }
                return string.Format("{0} {1}", dateTime.ToString("d", CultureInfo.CurrentCulture), dateTime.ToString("t", CultureInfo.CurrentCulture));
            }
            return DEFAULT_RETURN;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime();
        }
    }
}
