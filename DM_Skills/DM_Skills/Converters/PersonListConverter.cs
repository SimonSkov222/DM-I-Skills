using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace DM_Skills.Converters
{
    class PersonListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var persons = values[0] as ObservableCollection<Models.PersonModel>;
            var title = values[1] as string;

            List<string> firstnames = new List<string>();

            if (persons != null)
            {
                foreach (var item in persons)
                {
                    if (item.Name != null && item.Name != "")
                    {
                        var names = item.Name.Split(' ');
                        firstnames.Add(names[0]);
                    }
                }
            }

            if ((persons != null && persons.Count == 0) || firstnames.Count == 0)
                return title;
            else
                return string.Join(", ", firstnames);
        }
        

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
