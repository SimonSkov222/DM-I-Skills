using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class MinusDoubleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return 0;
            }
            if (values.Length < 1)
            {
                return 0;
            }

            double result = GetValue(values[0], parameter);

            for (int i = 1; i < values.Length; i++)
            {
                result -= GetValue(values[i], parameter);
            }

            return result;
        }

        private double GetValue(object value, object parameter)
        {
            if (value == null)
            {
                return 0;
            }
            if (!(value is double) && !(value is Thickness))
            {
                return 0;
            }
            if (parameter.ToString().ToLower().Contains("height"))
            {
                if (value is double)
                {
                    return (double)value;
                }
                else if(value is Thickness)
                {
                    return ((Thickness)value).Top + ((Thickness)value).Bottom;
                }
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
