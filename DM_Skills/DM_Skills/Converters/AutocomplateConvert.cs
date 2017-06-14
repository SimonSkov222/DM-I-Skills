using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace DM_Skills.Converters
{
    /// <summary>
    /// Klassen bruger interface`et IMultiValueConverter
    /// klassen styre hvornår et textblock element i
    /// autocomplete listen skal være synlig
    /// </summary>
    class AutocomplateConvert : IMultiValueConverter
    {
        /// <summary>
        /// Values må kun indehold to strings herefter 
        /// tjekker vi om den første værdi indeholder
        /// noget fra den anden
        /// hvis den gør vil den sende tilbage at det element 
        /// denne converter bliver brugt på skal være synlig
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values.Length != 2) return Visibility.Collapsed;

            if (values[0] == null || values[1] == null) return Visibility.Collapsed;

            //samlign ikke med mellerum
            string str1 = values[0].ToString().ToLower().Replace(" ", "");
            string str2 = values[1].ToString().ToLower().Replace(" ", ""); 
            
            return str1.Contains(str2) ?  Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Bliver ikke brugt
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
