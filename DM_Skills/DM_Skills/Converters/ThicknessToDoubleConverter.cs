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
    class ThicknessToDoubleConverter : IValueConverter
    {
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine(value);
            int numb = Reverse ? -1 : 1;

            switch ((parameter as string).ToLower())
            {
                case "left":    return ((Thickness)value).Left * numb;
                case "top":     return ((Thickness)value).Top * numb;
                case "right":   return ((Thickness)value).Right * numb;
                case "bottom":  return ((Thickness)value).Bottom * numb;
                default:        return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
