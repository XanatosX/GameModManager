using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataConverter
{
    class DatetimeToStringConverter : IValueConverter
    {
        private const string DEFAULT_RETURN = "Never";

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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime();
        }
    }
}
