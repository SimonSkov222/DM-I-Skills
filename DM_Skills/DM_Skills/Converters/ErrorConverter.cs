﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DM_Skills.Converters
{
    class ErrorConverter : IMultiValueConverter
    {
        private string name;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("CC");
            if (parameter != null)
            {
                var ss = parameter.ToString().Split(',');
                parameter = ss.Length > 0 ? ss[0] : null;
                if (ss.Length > 1)
                    name = ss[1];
            }

            if (values == null)
            {
                return false;
            }
            if (values.Length != 2)
            {

                return false;
            }
            if (parameter != null && parameter.ToString() != "")
            {
                int id = 0;
                int.TryParse(parameter.ToString(), out id);
                return GetByIndex(values, id);

            }
            return GetByBool(values);
            
        }

        private bool GetByBool(object[] values)
        {
            if (!(values[0] is bool) || !(values[1] is bool))
            {

                return false;
            }


            bool hasData = (bool)values[0];
            bool canUpload = (bool)values[1];
            Console.WriteLine("{0}:: {1} === {2} :: R: {3}", name, hasData, canUpload, hasData && !canUpload);
            return hasData && !canUpload;
        }

        private bool GetByIndex(object[] values, int index)
        {
            if (!(values[0] is bool) || !(values[1] is int))
            {
                return false;
            }


            bool hasData = (bool)values[0];
            int errno = (int)values[1];

            Console.WriteLine("{0}:: {1} === {2} :: R: {3} : Bit: {4}, V:{5}", name, hasData, (errno & index) == index, hasData && (errno & index) == index, (errno & index), errno);
            return hasData && (errno & index) == index;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
