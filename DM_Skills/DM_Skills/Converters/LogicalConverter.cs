using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class LogicalConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            

            if (parameter.ToString().ToLower() == "or")
            {
                foreach (var i in values)
                {
                    if (i is bool && (bool)i)
                    {
                        return true;
                    }
                }
            }
            else if (parameter.ToString().ToLower() == "and")
            {
                foreach (var i in values)
                {
                    if (i is bool && !(bool)i)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
