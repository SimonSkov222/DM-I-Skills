using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace DM_Skills.Views
{
    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    public partial class Connection : UserControl
    {
        Models.SettingsModel Settings;
        public Connection()
        {
            Settings = (Models.SettingsModel)FindResource("Settings");
            InitializeComponent();
            txtIP.Text = ServerIP;
            var p = new PasswordBox();
            Console.WriteLine(p.Password.GetType());

            var r = new RichTextBox();
            
        }
        public string ServerIP { get { return Scripts.Helper.GetLocalIPv4(); } }

        private string clientIP = "";
        private string clientPort = "";
        private string serverPort = "";

        private void rClient_Checked(object sender, RoutedEventArgs e)
        {

            if (!this.IsLoaded)
            {
                return;
            }
            if (((RadioButton)sender).IsChecked ?? false)
            {
                serverPort = txtPort.Text;
                txtPort.Text = clientPort;
                txtIP.Text = clientIP;
            }
            else
            {
                clientPort = txtPort.Text;
                txtPort.Text = serverPort;
                
                clientIP = txtIP.Text;
                txtIP.Text = ServerIP;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id = Grid.GetColumn(sender as Button);
            
            //OpenFileDialog dlgOpen = new OpenFileDialog();
            //SaveFileDialog dlgSave = new SaveFileDialog();

            //dlgOpen.DefaultExt = ".db";
            //dlgOpen.Filter = "Text documents (.db)|*.db";

            //bool? openResult = dlgOpen.ShowDialog();
            //bool? saveResult = dlgSave.ShowDialog();


            //if (openResult == true)
            //{
            //    txtDB.Text = dlgOpen.FileName;
            //    filename = dlgOpen.FileName;
            //}


            FileDialog dlg;

            if (id == 0)
            {
                dlg = new OpenFileDialog();
            }

            else
            {
                dlg = new SaveFileDialog();
            }

            dlg.DefaultExt = ".sqlite";
            dlg.Filter = "SQLite (.sqlite)|*.sqlite";


            if (dlg.ShowDialog() ?? false)
            {
                string filename = dlg.FileName;
                var myLocalDB = Scripts.Database.GetLocalDB();

                myLocalDB.Update("Settings", "Value", filename, (object)"LocationDB");
                myLocalDB.Disconnect();

                MessageBox.Show(filename);
            }





        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.IsClient || Settings.IsServer)
            {
                Console.WriteLine("TODO: Indstillinger.Button_Start_Click");
                return;
            }

            int port = 0;

            if (!int.TryParse(txtPort.Text,out port))
            {
                Console.WriteLine("TODO: Indstillinger.Button_Start_Click");
                return;
            }

            Settings.Server = new Scripts.Server();
            Settings.Server.Start(port);
            Settings.IsServer = true;
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.IsServer)
            {
                Settings.Server.Stop();
                Settings.IsServer = false;
            }
        }

        private void Button_Tilslut_Click(object sender, RoutedEventArgs e)
        {
            int port = 0;
            if (!int.TryParse(txtPort.Text, out port))
            {
                Console.WriteLine("TODO: Indstillinger.Button_Start_Click");
                return;
            }

            if (!Settings.IsClient)
            {
                Settings.Client = new Scripts.Client();
                Settings.Client.Connect(txtIP.Text, int.Parse(txtPort.Text));
                Settings.IsClient = true;
            }
        }

        private void Button_Afbryd_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.IsClient)
            {
                Settings.Client.Disconnect();
                Settings.IsClient = false;
            }
        }
    }
}
