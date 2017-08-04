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
using System.Windows.Shapes;

namespace DM_Skills.Views
{
    /// <summary>
    /// Interaction logic for Projektor.xaml
    /// </summary>
    public partial class Projektor : Window
    {

        public Controls.TimerControl Timer { get; set; }

        public Projektor(Controls.TimerControl timer)
        {
            Timer = timer;
            InitializeComponent();



        }

        public void SetFullScreen(bool value)
        {

            var screen = System.Windows.Forms.Screen.FromHandle(
                new System.Windows.Interop.WindowInteropHelper(this).Handle);



            Console.WriteLine("-->{0}", Timer.DisplayTime);
            var nWin = new Projektor(Timer);
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
                    Console.WriteLine(screen.Primary);

                    Console.WriteLine(screen.Bounds);
                    Console.WriteLine(screen.WorkingArea);
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


    }
}
