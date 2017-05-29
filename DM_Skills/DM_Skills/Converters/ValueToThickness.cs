using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    public class ValueToThicknessConverter : IValueConverter
    {
        public bool UseOnLeft { get; set; }
        public bool UseOnTop { get; set; }
        public bool UseOnRight { get; set; }
        public bool UseOnBottom { get; set; }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            Thickness margin = new Thickness();
            double pro = parameter == null ? 100 : System.Convert.ToDouble(parameter); 

            if (UseOnLeft) margin.Left = (double)value * (pro / 100);
            if (UseOnTop) margin.Top = (double)value * (pro / 100);
            if (UseOnRight) margin.Right = (double)value * (pro / 100);
            if (UseOnBottom) margin.Bottom = (double)value * (pro / 100);

            return margin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
