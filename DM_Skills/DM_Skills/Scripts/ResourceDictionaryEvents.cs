using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DM_Skills.Scripts
{
    public partial class ResourceDictionaryEvents : ResourceDictionary
    {
        


        public ResourceDictionaryEvents() {
            InitializeComponent();
        }

        void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "")
                (sender as PasswordBox).Tag = "Show";
            else
                (sender as PasswordBox).Tag = "Hidden";
        }
    }
}
