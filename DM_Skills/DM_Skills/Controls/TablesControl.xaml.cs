using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for TablesControl.xaml
    /// </summary>
    public partial class TablesControl : UserControl
    {

        static int holdnr = 10;

        public string SkolensNavnpropB1
        {
            get { return (string)GetValue(Skolenavnproperty); }
            set { SetValue(Skolenavnproperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Skolenavnproperty =
            DependencyProperty.Register("SkolensNavnprop", typeof(string), typeof(TablesControl), new PropertyMetadata("1"));



        public string KlasseNavnpropB1
        {
            get { return (string)GetValue(KlasseNavnpropB1Property); }
            set { SetValue(KlasseNavnpropB1Property, value); }
        }

        // Using a DependencyProperty as the backing store for KlasseNrpropB1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KlasseNavnpropB1Property =
            DependencyProperty.Register("KlasseNrpropB1", typeof(string), typeof(TablesControl), new PropertyMetadata("2"));



        public int HoldNrpropB1
        {
            get { return (int)GetValue(HoldNrpropB1Property); }
            set { SetValue(HoldNrpropB1Property, value); }
        }

        // Using a DependencyProperty as the backing store for HoldNrpropB1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoldNrpropB1Property =
            DependencyProperty.Register("HoldNrpropB1", typeof(int), typeof(TablesControl), new PropertyMetadata(1));



        public string DeltagerepropB1
        {
            get { return (string)GetValue(DeltagereB1propProperty); }
            set { SetValue(DeltagereB1propProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeltagereB1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeltagereB1propProperty =
            DependencyProperty.Register("DeltagerepropB1", typeof(string), typeof(TablesControl), new PropertyMetadata("4"));



        public string TidpropB1
        {
            get { return (string)GetValue(TidpropB1Property); }
            set { SetValue(TidpropB1Property, value); }
        }

        // Using a DependencyProperty as the backing store for TidB1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TidpropB1Property =
            DependencyProperty.Register("TidpropB1", typeof(string), typeof(TablesControl), new PropertyMetadata("5"));








        public TablesControl()
        {
            InitializeComponent();
            
        }
    }
}
