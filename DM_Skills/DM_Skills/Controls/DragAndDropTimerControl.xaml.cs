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

            var txtNumber = new TextBlock();
            var txtTime = new TextBlock();
            if (time != null)
            {
                listPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                
                txtNumber.Text = rowNumber + 1 + ".";
                Grid.SetRow(txtNumber, rowNumber);
                Grid.SetColumn(txtNumber, 0);
                

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


                txtTime.MouseDown += TxtTime_MouseMove;
            }


        }

        private void TxtTime_MouseMove(object sender, MouseEventArgs e)
        {
            DragDrop.DoDragDrop((sender as TextBlock), (sender as TextBlock).Text, DragDropEffects.All);
        }

        public void Reset()
        {

            listPanel.RowDefinitions.Clear();
            listPanel.Children.Clear();
            rowNumber = 0;

        }
    }
}
