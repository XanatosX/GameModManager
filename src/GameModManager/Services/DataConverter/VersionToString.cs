using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModManager.Services.DataConverter
{
    class VersionToString : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Version version)
            {
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            return string.Empty;
        }

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
