using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace DM_Skills.Converters
{
    class OvertimeConverter : IMultiValueConverter
    {
        public Brush Foreground_True { get; set; }
        public Brush Foreground_False { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString().ToLower())
            {
                case "visibility":
                    return IsInOverTime(values) ? Visibility.Visible : Visibility.Collapsed;
                case "color":
                    return IsInOverTime(values) ? Foreground_True : Foreground_False;
            }

            return null;
        }

        private bool IsInOverTime(object[] values)
        {

            if (values == null)
                return false;
            if (values[0] == null || values[1] == null)
                return false;
            if (!(values[0] is TimeSpan) || !(values[1] is int))
                return false;

            if (((TimeSpan)values[0]).TotalSeconds > (int)values[1] * 60)
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
