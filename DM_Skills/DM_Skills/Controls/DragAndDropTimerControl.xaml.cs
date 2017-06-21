using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Denne klasse arver fra UserControl
    /// Klassen styre hvad der skal ske i vores Tid liste
    /// </summary>
    public partial class DragAndDropTimerControl : UserControl
    {
        int rowNumber = 0;

        /// <summary>
        /// Skal være har da den opretter de elementer
        /// der er i xaml
        /// </summary>
        public DragAndDropTimerControl()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Tilføjer en tid til listen
        /// vi bruger en converter til at få den
        /// format vi gerne vil have
        /// </summary>
        public void Add(TimeSpan time)
            {
            //Top element
            if (time == null) return;

            listPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Position nummer
            var txtNumber = new TextBlock();
            txtNumber.Text = rowNumber + 1 + ".";
            Grid.SetRow(txtNumber, rowNumber);
            Grid.SetColumn(txtNumber, 0);

            //Vis tid
            var txtTime = new TextBlock();
            txtTime.SetBinding(TextBlock.TextProperty, new Binding()
            {
                Source = time,
                Converter = (IValueConverter)FindResource("TimeToStringConvert")
            });
            Grid.SetRow(txtTime, rowNumber);
            Grid.SetColumn(txtTime, 1);

            //Tilføj til view
            listPanel.Children.Add(txtNumber);
            listPanel.Children.Add(txtTime);

            rowNumber++;


            txtTime.MouseDown += TxtTime_MouseMove;
        }

        /// <summary>
        /// Fjerner de tider vi kan se i view
        /// </summary>
        public void Reset()
        {
            listPanel.RowDefinitions.Clear();
            listPanel.Children.Clear();
            rowNumber = 0;
        }

        /// <summary>
        /// Når man trækker i en tid i listen kan man 
        /// flytte teksten hent i en textbox 
        /// </summary>
        private void TxtTime_MouseMove(object sender, MouseEventArgs e)
        {
            DragDrop.DoDragDrop((sender as TextBlock), (sender as TextBlock).Text, DragDropEffects.All);
        }

    }
}
