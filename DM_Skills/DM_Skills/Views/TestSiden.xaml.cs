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
    /// Interaction logic for TestSiden.xaml
    /// </summary>
    public partial class TestSiden : Window
    {

        private Scripts.JsonServer server;
        private Scripts.JsonClient client;

        public TestSiden()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            server = new Scripts.JsonServer(new Scripts.JsonObjectConverter(), new Scripts.DataReceivedServer());
            server.Debug_Output += o => Application.Current.Dispatcher.Invoke(delegate() { sOutput.Text += o + "\n";  });
            server.Start(7788);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            client = new Scripts.JsonClient(new Scripts.JsonObjectConverter(), new Scripts.DataReceivedServer());
            client.Debug_Output += o => Application.Current.Dispatcher.Invoke(delegate () { cOutput.Text += o + "\n"; });
            client.Connect("127.0.0.1", 7788);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            client.Send((int)Scripts.JsonCommandIDs.Broadcast_LocationChanged, new Models.LocationModel() { Name = "Ballerup" });
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            server.Broadcast(127, new Models.PersonModel() { ID= 22, Name = "Bob Hansen", TeamID = 3});
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            server.Stop();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            sOutput.Text = "";
            cOutput.Text = "";
        }
    }
}
