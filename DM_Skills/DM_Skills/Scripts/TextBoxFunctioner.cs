using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DM_Skills.Scripts
{
    public partial class TextBoxFunctioner : ResourceDictionary
    {

        public TextBoxFunctioner()
        {
            InitializeComponent();
        }

        void OnTextChanged(object sender, RoutedEventArgs e)
        {
            
            string richText = new TextRange((sender as RichTextBox).Document.ContentStart, (sender as RichTextBox).Document.ContentEnd).Text;

            if (string.IsNullOrWhiteSpace(richText) && Regex.Matches(richText, "\\n").Count <= 1 && !richText.Contains(" "))
                (sender as RichTextBox).Tag = "Show";
            else
                (sender as RichTextBox).Tag = "Hidden";

            Console.WriteLine("asd");
            
            

        }
    }
}
