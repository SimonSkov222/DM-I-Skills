using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class ValueToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("Convert ");
            switch (parameter.ToString().ToLower())
            {
                case "listtoobservablecollection":
                    var result = new System.Collections.ObjectModel.ObservableCollection<Models.PersonModel>();

                    foreach (var item in (List<Models.PersonModel>)value)
                    {
                        result.Add(item);
                    }

                    return result;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("Convert back");
            switch (parameter.ToString().ToLower())
            {
                case "listtoobservablecollection":
                    var result = new List<Models.PersonModel>();

                    foreach (var item in (System.Collections.ObjectModel.ObservableCollection<Models.PersonModel>)value)
                    {
                        result.Add(item);
                    }

                    return result;
                default:
                    return null;
            }
        }
    }
}
