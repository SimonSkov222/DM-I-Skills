using System;
using System.Windows;
using System.Windows.Data;

namespace DM_Skills.Converters
{

    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { }

    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }
        


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            //if (OnChange != null) {
            //    OnChange();
            //}

            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
}
