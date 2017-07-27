using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class CompareValuesBoolConverter : CompareValuesConverter<bool> { };

    class CompareValuesConverter<T> : IValueConverter, IMultiValueConverter
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] paramSplit = ((string)parameter).Split(' ', ',');
            bool isEqual = System.Convert.ToBoolean(paramSplit[1]); ;
            bool result = false;

            if (value is double)
            {
                double paramVal = System.Convert.ToDouble(paramSplit[0]);
                result = ((double)value == paramVal && isEqual) || ((double)value != paramVal && !isEqual);
            }

            return result ? TrueValue : FalseValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            parameter = parameter == null ? "" : parameter;

            if (((string)parameter).ToUpper() == "OR")
            {
                foreach (var item in values)
                {
                    result = (bool)item;

                    if (result)
                        break;
                }
            }
            else if (((string)parameter).ToUpper() == "AND")
            {
                result = values.Length > 0;

                foreach (var item in values)
                {
                    result = result && (bool)item;
                }
            }

            return result ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
