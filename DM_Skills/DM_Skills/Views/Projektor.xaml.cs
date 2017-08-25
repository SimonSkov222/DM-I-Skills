using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DM_Skills.Views
{
    /// <summary>
    /// Interaction logic for Projektor.xaml
    /// </summary>
    public partial class Projektor : Window, INotifyPropertyChanged
    {

        private Models.SettingsModel Settings;
        public bool ClosedByFullScreen = false;
        public Controls.TimerControl Timer { get; set; }
        public MainWindow Parent = null;
        private DispatcherTimer updateTime;

        public Models.TableModelN BestTime { get { return _BestTime; } set { _BestTime = value; NotifyPropertyChanged(); } }
        public Models.TableModelN BestTimeToDay { get { return _BestTimeToDay; } set { _BestTimeToDay = value; NotifyPropertyChanged(); } }
        private Models.TableModelN _BestTime;
        private Models.TableModelN _BestTimeToDay;
        public string Date { get { return DateTime.Now.ToShortDateString(); } }


        public Projektor(Controls.TimerControl timer, MainWindow parent)
        {
            Timer = timer;
            Parent = parent;
            InitializeComponent();
            Closed += (oo, ee) => { if (!ClosedByFullScreen) Parent.Menu_Projektor.IsChecked = false; };
            Settings = FindResource("Settings") as Models.SettingsModel;
            Settings.OnUpload += UpdateData;

            updateTime = new DispatcherTimer();

            updateTime.Interval = new TimeSpan(0,0,15);
            updateTime.Tick += (o, e) => UpdateData();
            Loaded += (o, e) => updateTime.Start();
            Closed += (o, e) => updateTime.Stop();

            Loaded += (o, e) =>
            {
                UpdateData();
            };

            
            Parent.Closed += (o, e) =>
            {
                if (this != null)
                {
                    Close();
                }
            };

        }

        private void UpdateData()
        {
            BestTime = Models.TableModelN.GetBestTime(null, Settings.Location);
            BestTimeToDay = Models.TableModelN.GetBestTime(DateTime.Now, Settings.Location);
        }

        public void SetFullScreen(bool value)
        {
            ClosedByFullScreen = true;
            var screen = System.Windows.Forms.Screen.FromHandle(
                new System.Windows.Interop.WindowInteropHelper(this).Handle);

            
            var nWin = new Projektor(Timer, Parent);
            nWin.Timer = this.Timer;

            if (value)
            {
                object[] values = {
                    this.ActualHeight,
                    this.ActualWidth,
                    this.Left,
                    this.Top
                };

                if (screen != null)
                {
                    var workingArea = screen.Bounds;
                        nWin.Left = workingArea.Left;
                        nWin.Top = workingArea.Top;
                        nWin.Width = workingArea.Width;
                        nWin.Height = workingArea.Height;
                    
                        nWin.Tag = values;
                        nWin.WindowStyle = WindowStyle.None;
                        nWin.AllowsTransparency = true;
                        

                }
            }

            else if (this.Tag != null)
            {

                nWin.Height = (double)((object[])this.Tag)[0];
                nWin.Width = (double)((object[])this.Tag)[1];
                nWin.Left = (double)((object[])this.Tag)[2];
                nWin.Top = (double)((object[])this.Tag)[3];


                //this.WindowStyle = WindowStyle.SingleBorderWindow;
                ////this.AllowsTransparency = false;
                //this.WindowState = WindowState.Normal;
            }

            

            nWin.Show();
            Parent.projek = nWin;
            this.Close();
        }

        private void Controller_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetFullScreen(!GetFullScreen());
        }



        public bool GetFullScreen()
        {
            //Loaded += (o, e) => { WindowStyle == WindowStyle.None; };


            return WindowStyle == WindowStyle.None;
        }

        private void Controller_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                SetFullScreen(true);
        }

        private void Controller_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F11:
                    SetFullScreen(!GetFullScreen());
                    break;
                case Key.Escape:
                    if (GetFullScreen())
                        SetFullScreen(false);
                    break;
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Label_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            UpdateData();
        }
    }
}
