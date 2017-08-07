using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Denne klasse arver fra UserControl
    /// Klassen styre hvad der skal ske i vores Tid liste
    /// </summary>
    public partial class DragAndDropTimerControl : UserControl
    {



        public int Overtime
        {
            get { return (int)GetValue(OvertimeProperty); }
            set { SetValue(OvertimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Overtime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OvertimeProperty =
            DependencyProperty.Register(
                "Overtime", 
                typeof(int), 
                typeof(DragAndDropTimerControl), 
                new PropertyMetadata(1 ,new PropertyChangedCallback(CallBackProperty))
            );



        public bool IsDraggingItem
        {
            get { return (bool)GetValue(IsDraggingItemProperty); }
            private set { SetValue(IsDraggingItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDraggingItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDraggingItemProperty =
            DependencyProperty.Register("IsDraggingItem", typeof(bool), typeof(DragAndDropTimerControl), new PropertyMetadata(false));

        
        private IValueConverter timespanConverter;

        public TimeSpan OverTimeSpan { get; set; }


        public static void CallBackProperty(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((DragAndDropTimerControl)sender).OverTimeSpan = new TimeSpan(0, (int)e.NewValue, 0);
        }

        /// <summary>
        /// Skal være har da den opretter de elementer
        /// der er i xaml
        /// </summary>
        public DragAndDropTimerControl()
        {
            InitializeComponent();
            timespanConverter = (IValueConverter)FindResource("TimeToStringConvert");
            OverTimeSpan = new TimeSpan(0, Overtime, 0);
        }


        public void AddOverTimeLabel(TimeSpan time)
        {
            //Top element
            if (time == null) return;

            int rowID = listPanel.RowDefinitions.Count;
            listPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Position nummer
            var txtNumber = new TextBlock();
            var txtTime = BuildTimeLabel(time);


            //Placering
            Grid.SetRow(txtTime, rowID);
            Grid.SetColumn(txtTime, 1);
            Grid.SetRow(txtNumber, rowID);
            Grid.SetColumn(txtNumber, 0);

            //Styling
            txtNumber.Text = string.Format("{0}.", rowID + 1);
            txtNumber.VerticalAlignment = VerticalAlignment.Center;
            txtNumber.FontWeight = FontWeights.Bold;
            txtNumber.HorizontalAlignment = HorizontalAlignment.Right;


            //Tilføj til view
            listPanel.Children.Add(txtNumber);
            listPanel.Children.Add(txtTime);
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

            int rowID = listPanel.RowDefinitions.Count;
            listPanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Position nummer
            var txtNumber = new TextBlock();
            var txtTime = BuildTimeLabel(time);


            //Placering
            Grid.SetRow(txtTime, rowID);
            Grid.SetColumn(txtTime, 1);
            Grid.SetRow(txtNumber, rowID);
            Grid.SetColumn(txtNumber, 0);

            //Styling
            txtNumber.Text = string.Format("{0}.", rowID + 1);
            txtNumber.VerticalAlignment = VerticalAlignment.Center;
            txtNumber.FontWeight = FontWeights.Bold;
            txtNumber.HorizontalAlignment = HorizontalAlignment.Right;

            
            //Tilføj til view
            listPanel.Children.Add(txtNumber);
            listPanel.Children.Add(txtTime);
        }

        private Label BuildTimeLabel(object content) {
            var label = new Label();

            //Nicenis er et library der gør at når vi trækker i et element vil vi kunne se elementet
            Nicenis.Windows.DragSource.SetAllowDrag(label, true);
            Nicenis.Windows.DragSource.SetData(label, label);
            Nicenis.Windows.DragSource.SetAllowedEffects(label, DragDropEffects.Move);
            Nicenis.Windows.DragSource.SetVisualFeedbackOpacity(label, 0.8);


            Nicenis.Windows.DragSource.AddGiveFeedbackHandler(label, (o,e) => {
                Mouse.SetCursor(Cursors.Hand);
                e.Handled = true;
            });
            Nicenis.Windows.DragSource.AddPreviewDraggedHandler(label, (o, e) => { IsDraggingItem = false; });
            Nicenis.Windows.DragSource.AddDraggingHandler(label, (o, e) => { IsDraggingItem = true; });


            label.SetBinding(Nicenis.Windows.DragSource.VisualFeedbackOffsetProperty, new Binding() {
                Path = new PropertyPath(Nicenis.Windows.DragSource.ContactPositionProperty),
                RelativeSource = RelativeSource.Self
            });

            label.SetBinding(Label.ContentProperty, new Binding()

            {
                Source = content,
                Converter = timespanConverter
            });
            label.Style = (Style)FindResource("Label_Style_Default");

            return label;
        }


        /// <summary>
        /// Fjerner de tider vi kan se i view
        /// </summary>
        public void Reset()
        {
            listPanel.RowDefinitions.Clear();
            listPanel.Children.Clear();
        }
    }
}
