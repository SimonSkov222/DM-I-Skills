using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
    public partial class Connection : UserControl, INotifyPropertyChanged
    {
        Models.SettingsModel Settings;
        public Connection()
        {
            Settings = (Models.SettingsModel)FindResource("Settings");
            Application.Current.MainWindow.Closed += MainWindow_Closed;
            Loaded += Connection_Loaded;
            InitializeComponent();

        }

        private void Connection_Loaded(object sender, RoutedEventArgs e)
        {
            var myLocalDB = Scripts.Database.GetLocalDB();
            
            


            
            Settings.OverTimeMin        = Convert.ToInt32(myLocalDB.GetRow("Settings","Value", "WHERE `Name`='OverTime'")[0]);
            Settings.TableCnt           = Convert.ToInt32(myLocalDB.GetRow("Settings", "Value", "WHERE `Name`='TableCount'")[0]);
            serverPort                  = Convert.ToString(myLocalDB.GetRow("Settings","Value", "WHERE `Name`='ServerPort'")[0]);
            clientIP                    = Convert.ToString(myLocalDB.GetRow("Settings","Value", "WHERE `Name`='ClientIP'")[0]);
            clientPort                  = Convert.ToString(myLocalDB.GetRow("Settings", "Value", "WHERE `Name`='ClientPort'")[0]);

            myLocalDB.Disconnect();

            


            txtIP.Text = ServerIP;
            txtPort.Text = serverPort;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            var myLocalDB = Scripts.Database.GetLocalDB();

            myLocalDB.Update("Settings", "Value", Settings.OverTimeMin, "WHERE `Name`='OverTime'");
            myLocalDB.Update("Settings", "Value", Settings.TableCnt, "WHERE `Name`='TableCount'");
            myLocalDB.Update("Settings", "Value", serverPort, "WHERE `Name`='ServerPort'");
            myLocalDB.Update("Settings", "Value", clientIP, "WHERE `Name`='ClientIP'");
            myLocalDB.Update("Settings", "Value", clientPort, "WHERE `Name`='ClientPort'");

            myLocalDB.Disconnect();

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

            if (Settings.IsServer || Settings.IsClient)
            {
                return;
            }
            int id = Grid.GetColumn(sender as Button);

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
                Settings.FileNameDB = dlg.FileName;
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

            try
            {
               System.IO.Path.GetFullPath(Settings.FileNameDB);
            }
            catch (Exception)
            {

                MessageBox.Show("Stien for databasen findes ikke", "Ingen database", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(Settings.FileNameDB))
            {
                if (!Directory.Exists(Settings.FileNameDB))
                {
                    //MessageBox.Show("Stien for databasen findes ikke","Ingen database",MessageBoxButton.OK,MessageBoxImage.Error);
                    //return;
                }
            }

            //Gem db location
            var myLocalDB = Scripts.Database.GetLocalDB();
            myLocalDB.Update("Settings", "Value", Settings.FileNameDB, (object)"LocationDB");
            myLocalDB.Disconnect();
            
            Scripts.Database.CreateDatabase();

            Settings.Server = new Scripts.Server();
            Settings.Server.Start(port);


            var r = new Random();

            //for (int i = 0; i < 25; i++)
            //{
            //    var d = new Models.TableModelN();
            //    d.Team.Class = "7z" + i;
            //    d.Team.Date = $"{r.Next(15, 17).ToString().PadLeft(2, '0')}-{r.Next(8, 8)}-{r.Next(2017, 2017)}";
            //    d.Team.Time = $"{r.Next(10, 30)}:{r.Next(10, 59)}:{r.Next(10, 59)}";
            //    d.School.Name = $"Hersted {r.Next(100)} Skole";
            //    d.Location.Name = "ballerup";
            //    d.Persons.Add(new Models.PersonModel() { Name = "hej" });
            //    d.Persons.Add(new Models.PersonModel() { Name = "meh" });

            //    d.Upload();
            //    d = null;
            //}

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

            if (!Settings.IsClient && !Settings.IsServer)
            {
                Settings.Client = new Scripts.Client();
                Settings.Client.Connect(txtIP.Text, int.Parse(txtPort.Text));
            }
        }

        private void Button_Afbryd_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.IsClient)
            {
                Settings.Client.Disconnect();
            }
        }

        private void Button_SchoolUpload_Click(object sender, RoutedEventArgs e)
        {
            string richText = new TextRange(txtSkoleList.Document.ContentStart, txtSkoleList.Document.ContentEnd).Text;

            var schoolList = Regex.Matches(richText, ".+?(?:\\n|$)", RegexOptions.Singleline)
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .Distinct();


            foreach (var i in schoolList)
            {
                string name = i.Trim().Replace("\n", "");
                if (name.Length > 0)
                {
                    (new Models.SchoolModel() { Name = name }).Upload();
                }
            }
            txtSkoleList.Document.Blocks.Clear();

            Settings.NotifyPropertyChanged(nameof(Settings.AllSchools));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var list = Models.SchoolModel.GetAll();

            foreach (var i in list)
            {
                Console.WriteLine(i.Name);
            }
        }

        private void Button_DeleteSchools_Click(object sender, RoutedEventArgs e)
        {
            Models.SchoolModel.RemoveUnused();
            Settings.NotifyPropertyChanged(nameof(Settings.AllSchools));
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SaveToLocal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender as TextBox).IsKeyboardFocused)
            {
                return;
            }

            if (rServer.IsChecked ?? false)
            {
                serverPort = txtPort.Text;
            }
            else
            {
                clientIP = txtIP.Text;
                clientPort = txtPort.Text;
            }
        }

        private void ComboBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0)
            {
                (sender as ComboBox).SelectedIndex = 0;
            }
        }

        private void Button_Backup_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.DefaultExt = ".sqlite";
            dlg.Filter = "SQLite (.sqlite)|*.sqlite";

            if (dlg.ShowDialog() == true)
            {
                var fileName = dlg.FileName;
                System.IO.File.Copy(Settings.FileNameDB, dlg.FileName);
            }
        }
    }
}
