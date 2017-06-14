using System;
using System.Globalization;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    /// <summary>
    /// Klassen bruger interface`et IValueConverter
    /// Klassen giver os et procent del af en samlet værdi
    /// </summary>
    class ProcentOfValueConverter : IValueConverter
    {
        /// <summary>
        /// Den giver os en procent del af den samlet værdi
        /// procent delen blivet angivet som parameter
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double procent = System.Convert.ToDouble(parameter);
            double number = System.Convert.ToDouble(value);
            
            return number / 100 * procent;
        }

        /// <summary>
        /// Bruges ikke
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
