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
    class DragAndDropTimerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values == null)
                return Visibility.Collapsed;
            if (values[0] == null || values[1] == null)
                return Visibility.Collapsed;
            if (!(values[0] is TimeSpan) || !(values[1] is int))
                return Visibility.Collapsed;

            if(((TimeSpan)values[0]).TotalSeconds > (int)values[1] * 60)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }


        

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
