using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DM_Skills.Converters
{

    public class BoolToBrushConverter : BoolToValueConverter<Brush> { }
    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { }

    /// <summary>
    /// Klassen bruger interface`et IValueConverter
    /// Klassen gør det muligt at lave en bool en to værdi
    /// af en anden type alt efter om bool er true eller false
    /// </summary>
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }
        
        /// <summary>
        /// Laver bool om en en anden type
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Laver en type om til bool
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
}
