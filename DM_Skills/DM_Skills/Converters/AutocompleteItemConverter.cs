using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    /// <summary>
    /// Klassen bruger interface`et IValueConverter
    /// Denne gør det muligt at bruge en List<string>
    /// som Children i autocomplete
    /// </summary>
    class AutocompleteItemConverter : IValueConverter
    {
        /// <summary>
        /// Skal convert en List af string om til 
        /// en ObservableCollection af string
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new System.Collections.ObjectModel.ObservableCollection<string>((List<string>)value);
        }

        /// <summary>
        /// Bliver ikke brugt
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
