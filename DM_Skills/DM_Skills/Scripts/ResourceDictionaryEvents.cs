using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Scripts
{
    public partial class ResourceDictionaryEvents : ResourceDictionary
    {

        void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Changes");
        }
    }
}
