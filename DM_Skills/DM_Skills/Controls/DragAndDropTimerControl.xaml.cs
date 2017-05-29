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
    /// Interaction logic for DragAndDropTimerControl.xaml
    /// </summary>
    public partial class DragAndDropTimerControl : UserControl
    {
        public DragAndDropTimerControl()
        {
            InitializeComponent();

        }
        int rowNumber = 0;
        public void Add(TimeSpan time)
        {

            if (time != null)
            {
                listPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                var txtNumber = new TextBlock();
                txtNumber.Text = rowNumber.ToString() + ".";
                Grid.SetRow(txtNumber, rowNumber);
                Grid.SetColumn(txtNumber, 0);
                var txtTime = new TextBlock();

                txtTime.SetBinding(TextBlock.TextProperty, new Binding()
                {
                    Source = time,
                    Converter = (IValueConverter)FindResource("TimeToStringConvert")
                });
                Grid.SetRow(txtTime, rowNumber);
                Grid.SetColumn(txtTime, 1);

                listPanel.Children.Add(txtNumber);
                listPanel.Children.Add(txtTime);

                rowNumber++;
            }

        }

        private void btn_removeRow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
