//1.3 TimerControl
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;

namespace DM_Skills.Controls
{

    public partial class TimerControl : UserControl, INotifyPropertyChanged
    {
//1.3.0 Attributes
public event Action OnStart;
public event Action OnStop;
public event Action<TimeSpan> OnLap;
public event Action OnReset;
public event PropertyChangedEventHandler PropertyChanged; //INotifyPropertyChanged

private DispatcherTimer EventTimer;
private Stopwatch _Watch = new Stopwatch();
public TimeSpan DisplayTime { get { return _Watch.Elapsed; } }

//1.3.1 TimerControl()
public TimerControl()
{
    InitializeComponent();
    EventTimer = new DispatcherTimer();
    EventTimer.Interval = TimeSpan.FromMilliseconds(1);
    EventTimer.Tick += (o, e) => { NotifyPropertyChanged("DisplayTime"); };
}

//1.3.2 NotifyPropertyChanged()
private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
{
    //Tjekker om PropertyChanged er sat og kalder den hvis den er
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

//1.3.3 Reset()
public void Reset()
{
    Button_TimeControl_Click(btn_Reset, null);
}

//1.3.4 Button_TimeControl_Click()
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
        case 2: OnLap?.Invoke(_Watch.Elapsed); break;
        case 3: OnReset?.Invoke(); break;
    }

}

    }
}