using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace DM_Skills.Converters
{
    class AutocomplateConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values.Length != 2) return Visibility.Collapsed;

            if (values[0] == null || values[1] == null) return Visibility.Collapsed;

            string str1 = values[0].ToString().ToLower().Replace(" ", "");
            string str2 = values[1].ToString().ToLower().Replace(" ", ""); ;
            
            return str1.Contains(str2) ?  Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
