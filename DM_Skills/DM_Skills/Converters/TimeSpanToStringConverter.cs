using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class TimeSpanToStringConverter : IValueConverter
    {
        /// <summary>
        /// Lav en TimeSpan om til en læselig tekst
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Hent tallet og gør hvis det er mindre en 10 at der er 0 foran
            string minutes      = ((TimeSpan)value).Minutes.ToString().PadLeft(2, '0');
            string seconds      = ((TimeSpan)value).Seconds.ToString().PadLeft(2, '0');
            string milliseconds = (((TimeSpan)value).Milliseconds/10).ToString().PadLeft(2, '0'); //dividere 10 for at fjerne et 0

            return string.Format("{0}:{1}:{2}", minutes, seconds, milliseconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
