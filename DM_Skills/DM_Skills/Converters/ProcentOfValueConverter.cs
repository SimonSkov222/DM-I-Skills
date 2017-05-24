using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class ProcentOfValueConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Det tal der skal beregnes ud fra</param>
        /// <param name="parameter">Procent f.eks 10% = int 10</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            double procent = System.Convert.ToDouble(parameter);
            double number = System.Convert.ToDouble(value);

            Console.WriteLine(number);
            return number / 100 * procent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
