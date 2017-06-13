using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;
using System.Diagnostics;

namespace DM_Skills.Controls
{
    /// <summary>
    /// Interaction logic for TimerControl.xaml
    /// </summary>
    public partial class TimerControl : UserControl, INotifyPropertyChanged
    {

        

        /****************************
         * 
         *      Properties
         * 
         ***************************/

        public event Action OnStart;
        public event Action OnStop;
        public event Action<TimeSpan> OnLap;
        public event Action OnReset;

        private DispatcherTimer EventTimer;
        private Stopwatch _Watch = new Stopwatch();
        public TimeSpan DisplayTime { get { return _Watch.Elapsed; } }

        /****************************
         * 
         *      Interface: INotifyPropertyChanged
         * 
         ***************************/
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        



        public TimerControl()
        {
            InitializeComponent();
            EventTimer = new DispatcherTimer();
            EventTimer.Interval = TimeSpan.FromMilliseconds(1);
            EventTimer.Tick += Timer_Tick;
            
        }

        public void Reset()
        {
            Button_TimeControl_Click(btn_Reset, null);
        }


        /****************************
         * 
         *      Events
         * 
         ***************************/
        private void Button_TimeControl_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            if ((sender as Button).Name == btn_Start.Name) id = 0;
            else if ((sender as Button).Name == btn_Stop.Name) id = 1;
            else if ((sender as Button).Name == btn_Lap.Name) id = 2;
            else if ((sender as Button).Name == btn_Reset.Name) id = 3;

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
                case 0: OnStart?.Invoke();              break;
                case 1: OnStop?.Invoke();               break;
                case 2: OnLap?.Invoke(_Watch.Elapsed);  break;
                case 3: OnReset?.Invoke();              break;
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NotifyPropertyChanged("DisplayTime");
        }
    }   
}       
           