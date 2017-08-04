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
            Console.WriteLine(values.Length);
            Console.WriteLine(parameter);

            switch (parameter.ToString().ToLower())
            {
                case "text":
                    return GetText(values[0], values[1]);
                case "popup":
                    return IsPopupOpen(values[0], values[1]);
            }
            return null;
        }

        private bool IsPopupOpen(object height, object isOpen) {
            bool mHeight = (double)height > 0;
            bool open = (bool)isOpen;

            return open && mHeight;
        }
        private string GetText(object list, object placeholder)
        {
            var l = list as ObservableCollection<Models.PersonModel>;
            var p = placeholder as string;

            List<string> firstnames = new List<string>();

            if (l != null)
            {
                foreach (var item in l)
                {
                    if (item.Name != null && item.Name != "")
                    {
                        var names = item.Name.Split(' ');
                        firstnames.Add(names[0]);
                    }
                }
            }

            if ((l != null && l.Count == 0) || firstnames.Count == 0)
                return p;
            else
                return string.Join(", ", firstnames);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
