using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Input;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Klassen arver fra UserControl og bruger interface`et INotifyPropertyChanged
    /// Klassen styre hvordan stopuret skal fungere
    /// </summary>
    public partial class TimerControl : UserControl, INotifyPropertyChanged
    {
        public int Overtime
        {
            get { return (int)GetValue(OvertimeProperty); }
            set { SetValue(OvertimeProperty, value); }
        }

        public static readonly DependencyProperty OvertimeProperty =
            DependencyProperty.Register(
                "Overtime",
                typeof(int),
                typeof(TimerControl),
                new PropertyMetadata(1)
            );



        public event Action OnStart;
        public event Action OnStop;
        public event Action<TimeSpan> OnLap;
        public event Action OnReset;
        public event PropertyChangedEventHandler PropertyChanged; //INotifyPropertyChanged

        private DispatcherTimer EventTimer;
        private Stopwatch _Watch = new Stopwatch();
        public TimeSpan DisplayTime { get { return _Watch.Elapsed; } }

        private RadioButton rBtn_Forside;
        


        /// <summary>
        /// Opretter elementerne fra xaml samt laver 
        /// en timer der opdatere tiden i view
        /// </summary>
        public TimerControl()
        {
            InitializeComponent();
            EventTimer = new DispatcherTimer();
            EventTimer.Interval = TimeSpan.FromMilliseconds(1);
            EventTimer.Tick += (o, e) => { NotifyPropertyChanged("DisplayTime"); };
            Application.Current.MainWindow.PreviewKeyUp += MainWindow_KeyUp;
            rBtn_Forside = Application.Current.MainWindow.FindName("Menu_Forside") as RadioButton;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (_Watch.IsRunning)
            {
                if (e.Key == Key.Space)
                {
                    if (rBtn_Forside == null) {
                        return;
                    }

                    if (rBtn_Forside.IsChecked ?? false)
                    {
                        IInputElement focusedControl = Keyboard.FocusedElement;
                        if (!(focusedControl is TextBox))
                        {
                            Button_TimeControl_Click(btn_Lap, null);
                        }
                    }
                    
                    //IInputElement focusedControl = FocusManager.GetFocusedElement(this);
                    
                }
            }
        }


        /// <summary>
        /// Fortæller at en property har ændret sig
        /// </summary>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //Tjekker om PropertyChanged er sat og kalder den hvis den er
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Gør at man kan nulstille tiden 
        /// uden at trykke på knappen Nulstil
        /// </summary>
        public void Reset()
        {
            Button_TimeControl_Click(btn_Reset, null);
        }

        /// <summary>
        /// Når man trykker på en knap vil denne metode blive kaldt
        /// Denne metode finder frem til hvad knap man har trykket på
        /// og udføre det knappen skal gøre
        /// </summary>
        private void Button_TimeControl_Click(object sender, RoutedEventArgs e)
        {
            //Find knap id
            int id = -1;
            if ((sender as Button).Name == btn_Start.Name) id = 0;
            else if ((sender as Button).Name == btn_Stop.Name) id = 1;
            else if ((sender as Button).Name == btn_Lap.Name) id = 2;
            else if ((sender as Button).Name == btn_Reset.Name) id = 3;

            //Hvad skal knappen gøre
            switch (id)
            {
                case 0:     //Start tiden
                    EventTimer.Start();
                    _Watch.Start();
                    break;
                case 1:     //Stop tiden
                    _Watch.Stop();
                    EventTimer.Stop();
                    NotifyPropertyChanged("DisplayTime");
                    break;
                case 3:     //Stop og nulstil
                    EventTimer.Stop();
                    _Watch.Reset();
                    NotifyPropertyChanged("DisplayTime");
                    break;
            }

            //
            //  Kald evnet
            //
            switch (id)
            {
                case 0: OnStart?.Invoke(); break;
                case 1: OnStop?.Invoke(); break;
                case 2: if (_Watch.IsRunning) { OnLap?.Invoke(_Watch.Elapsed); } break;
                case 3: OnReset?.Invoke(); break;
            }

        }

    }
}