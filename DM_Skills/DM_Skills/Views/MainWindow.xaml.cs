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

namespace DM_Skills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(asd.SkolensNavnpropB1);
            MessageBox.Show(asd.KlasseNavnpropB1);
            MessageBox.Show(asd.DeltagerepropB1);
            MessageBox.Show(asd.TidpropB1);
        }




        private void TimerControl_OnLap(TimeSpan obj)
        {
            timeList.Add(obj);
        }
    }
}
