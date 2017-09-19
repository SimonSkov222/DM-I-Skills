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

        private long addTime = 0;
        public long AddTime
        {
            get { return addTime; }
            set
            {
                addTime = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DisplayTime");
            }
        }
        private DispatcherTimer EventTimer;
        private Stopwatch _Watch = new Stopwatch();

        public TimeSpan DisplayTime
        {
            get
            {
                var moreTime = TimeSpan.FromMilliseconds(addTime);
                return _Watch.Elapsed + moreTime;
            }
        }

        private bool HasLoadedOnce = false;




        /// <summary>
        /// Opretter elementerne fra xaml samt laver 
        /// en timer der opdatere tiden i view
        /// </summary>
        public TimerControl()
        {

            InitializeComponent();

            Models.SettingsModel.Singleton.OnGetTimeStatus = () =>
            {
                return "{ \"T\" : \""+ _Watch.Elapsed.TotalMilliseconds + "\", \"B\": "+ (_Watch .IsRunning ? 1 : 0) + " }";//new object[] { _Watch.Elapsed.TotalMilliseconds, false };
            };
            Models.SettingsModel.Singleton.OnSetTimeStatus = (msec, started) =>
            {
                AddTime = Convert.ToInt64(msec);
            };


            Models.SettingsModel.Singleton.OnTimerStopped += () =>
            {
                if (Models.SettingsModel.Singleton.IsClient && Models.SettingsModel.Singleton.UseGetTime)
                {
                    Button_Stop_Click(null, null);
                }
            };


            Models.SettingsModel.Singleton.OnTimerReset += () =>
            {
                if (Models.SettingsModel.Singleton.IsClient && Models.SettingsModel.Singleton.UseGetTime)
                {
                    Button_Reset_Click(null, null);
                }
            };

            Models.SettingsModel.Singleton.OnTimerStarted += () => 
            {
                if (Models.SettingsModel.Singleton.IsClient && Models.SettingsModel.Singleton.UseGetTime)
                {
                    addTime = 0;
                    Button_Start_Click(null, null);
                }
            };
            EventTimer = new DispatcherTimer();
            EventTimer.Interval = TimeSpan.FromMilliseconds(1);
            EventTimer.Tick += (o, e) => { NotifyPropertyChanged("DisplayTime"); };
            Loaded += (o, e) =>
            {
                if (!HasLoadedOnce && this.IsVisible)
                {
                    HasLoadedOnce = true;
                    Application.Current.MainWindow.PreviewKeyUp += MainWindow_KeyUp;
                }
            };
        }

        private void Settings_OnTimerStarted()
        {
            if (Models.SettingsModel.Singleton.IsClient)
            {
                Button_Start_Click(btn_Start, null);
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (_Watch.IsRunning)
            {
                if (e.Key == Key.Space)
                {

                    if (this.IsVisible)
                    {
                        IInputElement focusedControl = Keyboard.FocusedElement;
                        if (!(focusedControl is TextBox))
                        {
                            Button_Lab_Click(btn_Lap, null);
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
            if (!Models.SettingsModel.Singleton.IsClient && !Models.SettingsModel.Singleton.UseGetTime)
            {
                Button_Reset_Click(btn_Reset, null);
            }
        }



        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (_Watch.IsRunning) { return; }


            if (Models.SettingsModel.Singleton.IsServer)
            {
                Models.SettingsModel.Singleton.Server.BroadcastLine((int)Scripts.JsonCommandIDs.Broadcast_TimerStarted);

            }



            EventTimer.Start();
            _Watch.Start();

            OnStart?.Invoke();
        }



        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (!_Watch.IsRunning) { return; }

            if (Models.SettingsModel.Singleton.IsServer)
            {
                Models.SettingsModel.Singleton.Server.BroadcastLine((int)Scripts.JsonCommandIDs.Broadcast_TimerStopped);
            }

            _Watch.Stop();
            EventTimer.Stop();
            NotifyPropertyChanged("DisplayTime");

            OnStop?.Invoke();
        }
        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (Models.SettingsModel.Singleton.IsServer)
            {
                Models.SettingsModel.Singleton.Server.BroadcastLine((int)Scripts.JsonCommandIDs.Broadcast_TimerReset);

            }

            EventTimer.Stop();
            _Watch.Reset();
            AddTime = 0;
            NotifyPropertyChanged("DisplayTime");
        }

        private void Button_Lab_Click(object sender, RoutedEventArgs e)
        {
            if (!_Watch.IsRunning) { return; }
            OnLap?.Invoke(_Watch.Elapsed + TimeSpan.FromMilliseconds(addTime));
        }

        ///// <summary>
        ///// Når man trykker på en knap vil denne metode blive kaldt
        ///// Denne metode finder frem til hvad knap man har trykket på
        ///// og udføre det knappen skal gøre
        ///// </summary>
        //private void Button_TimeControl_Click(object sender, RoutedEventArgs e)
        //{
        //    //Find knap id
        //    int id = -1;
        //    if ((sender as Button).Name == btn_Start.Name) id = 0;
        //    else if ((sender as Button).Name == btn_Stop.Name) id = 1;
        //    else if ((sender as Button).Name == btn_Lap.Name) id = 2;
        //    else if ((sender as Button).Name == btn_Reset.Name) id = 3;

        //    //Hvad skal knappen gøre
        //    switch (id)
        //    {
        //        case 0:     //Start tiden
        //            EventTimer.Start();
        //            _Watch.Start();

        //            break;
        //        case 1:     //Stop tiden
        //            _Watch.Stop();
        //            EventTimer.Stop();
        //            NotifyPropertyChanged("DisplayTime");
        //            break;
        //        case 3:     //Stop og nulstil
        //            EventTimer.Stop();
        //            _Watch.Reset();
        //            AddTime = 0;
        //            NotifyPropertyChanged("DisplayTime");
        //            break;
        //    }

        //    //
        //    //  Kald evnet
        //    //
        //    switch (id)
        //    {
        //        case 0: OnStart?.Invoke(); break;
        //        case 1: OnStop?.Invoke(); break;
        //        case 2: if (_Watch.IsRunning) { OnLap?.Invoke(_Watch.Elapsed + TimeSpan.FromMilliseconds(addTime)); } break;
        //        case 3: OnReset?.Invoke(); break;
        //    }

        //}

    }
}