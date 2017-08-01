using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class AutocompleteConverter : IMultiValueConverter
    {
        public const string PARAMS_OPTION = "Option";
        public const string PARAMS_POPUP = "Popup";

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)parameter)
            {
                case PARAMS_POPUP:
                    return IsPopupOpen((ItemCollection)values[0], values[1].ToString());
                case PARAMS_OPTION:
                    return GetVisibilityForOption((string)values[0], values[1].ToString());
            }

            return null;
        }




        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool IsPopupOpen(ItemCollection items, string textbox)
        {

            var visibleItems = items.Cast<ListViewItem>().Where(o => o.Visibility == Visibility.Visible).ToArray();
            Console.WriteLine("V Item: {0}", visibleItems.Length);
            if (visibleItems.Length == 1) {
                return !visibleItems[0].Content.ToString().Equals(textbox);
            }
            if (visibleItems.Length > 0) {
                return true;
            }
            else {
                return false;
            }
        }



        private Visibility GetVisibilityForOption(string textbox, string option)
        {

            textbox = textbox.Replace(" ", "").ToLower();
            option = option.Replace(" ", "").ToLower();
            
            return option.StartsWith(textbox) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
