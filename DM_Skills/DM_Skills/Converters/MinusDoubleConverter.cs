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
                Console.WriteLine("Null");
                return 0;
            }
            if (values.Length < 1)
            {
                Console.WriteLine("< 1");
                return 0;
            }

            double result = GetValue(values[0], parameter);

            for (int i = 1; i < values.Length; i++)
            {
                result -= GetValue(values[i], parameter);
            }
            Console.WriteLine("R: {0}", result);

            return result;
        }

        private double GetValue(object value, object parameter)
        {
            if (value == null)
            {
                Console.WriteLine("V null");
                return 0;
            }
            if (!(value is double) && !(value is Thickness))
            {
                Console.WriteLine("V typy");
                return 0;
            }
            Console.WriteLine("p: {0}", parameter);
            if (parameter.ToString().ToLower().Contains("height"))
            {
                Console.WriteLine("V c");
                if (value is double)
                {
                Console.WriteLine("V d");
                    return (double)value;
                }
                else if(value is Thickness)
                {
                Console.WriteLine("V t");
                    return ((Thickness)value).Top + ((Thickness)value).Bottom;
                }
            }

                Console.WriteLine("V r");
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
