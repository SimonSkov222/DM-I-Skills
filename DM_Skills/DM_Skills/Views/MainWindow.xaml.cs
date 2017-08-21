using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SQLite_DB_LIB;                    //Database
using System.ComponentModel;            //INotifyPropertyChanged
using System.Runtime.CompilerServices;  //CallerMemberName for PropertyChanged
using System.Windows.Documents;
using System.Windows.Data;

namespace DM_Skills
{
    /// <summary>
    /// Klassen arver fra Window og bruger interface`et INotifyPropertyChanged
    /// Klassen styre det der skal ske i tidtagning fanen
    /// </summary>
    public partial class MainWindow : Window
    {

        public Views.Projektor projek;
        private Models.SettingsModel Settings;
        

        /// <summary>
        /// Her connecter vi til databasen og opretter den
        /// vi tilføjet også en load event
        /// </summary>
        /// 
        public MainWindow()
        {
            //Scripts.Database.CreateDatabase();
            Scripts.Database.CreateLocalDatabase();
            InitializeComponent();

            Settings = (Models.SettingsModel)FindResource("Settings");

            Settings.OnConnection += Settings_OnConnection;
            Settings.OnDisconnection += Settings_OnDisconnection;
            Loaded += (o, e) => { Menu_Indstillinger.IsChecked = false; Menu_Indstillinger.IsChecked = true; };

        }

        private void Settings_OnDisconnection(bool disconnectByUser)
        {
            Menu_Indstillinger.IsChecked = true;
        }

        private void Settings_OnConnection()
        {
            Console.WriteLine("Connection");
            Settings.NotifyPropertyChanged(nameof(Settings.AllLocations));
            Settings.NotifyPropertyChanged(nameof(Settings.AllSchools));
            Console.WriteLine("Skoler:");
            foreach (var item in Settings.AllSchools)
            {
                Console.WriteLine(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            Console.WriteLine("TablesControl Skoler:");
            foreach (var item in (view_forside.listOfTables.Children[0] as Controls.TablesControl).Schools)
            {
                Console.WriteLine(item);
            }
        }

        private void Menu_Projektor_Checked(object sender, RoutedEventArgs e)
        {
            if (Menu_Projektor.IsChecked ?? false)
            {
                projek = new Views.Projektor(view_forside.stopwatch, this);
                
                projek.Show();
            }
            else
            {
                projek.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var settings = FindResource("Settings") as Models.SettingsModel;
            settings.IsServer = true;
            settings.Server = new Scripts.Server();
            settings.Server.Start(7788);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var settings = FindResource("Settings") as Models.SettingsModel;
            settings.IsClient = true;
            settings.Client = new Scripts.Client();
            //settings.Client.Connect(txtIP.Text, 7788);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //var settings = FindResource("Settings") as Models.SettingsModel;
            //Models.SettingsModel.lab.Content += "Contact Server...\n";
            //Console.WriteLine("Contact Server...");
            //settings.Client.Send(Scripts.PacketType.Disconnect, (o) => { System.Threading.Thread.Sleep(1000); Console.WriteLine("CB"); }, "Hej");
            //Models.SettingsModel.lab.Content += "Contact Done\n";
            //Console.WriteLine("Contact Done");


        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            Scripts.Database.CreateDatabase();

            for (int i = 0; i < 10; i++)
            {
                var d = new Models.TableModelN();
                d.Team.Class = "7z"+i;
                d.Team.Date = $"{r.Next(1,28).ToString().PadLeft(2,'0')}-{r.Next(1, 12).ToString().PadLeft(2, '0')}-{r.Next(2016, 2018).ToString().PadLeft(2, '0')}";
                d.Team.Time = $"{r.Next(0,15).ToString().PadLeft(2,'0')}:{r.Next(0,59).ToString().PadLeft(2,'0')}:{r.Next(0,59).ToString().PadLeft(2,'0')}";
                d.School.Name = $"Hersted {r.Next(1, 10)} Skole";
                d.Location.Name = "ballerup";
                d.Persons.Add(new Models.PersonModel() { Name = "hej" });
                d.Persons.Add(new Models.PersonModel() { Name = "meh" });

                d.Upload();
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            var margin = warningDB.Margin;
            margin.Top = ((ScrollViewer)sender).VerticalOffset;
            warningDB.Margin = margin;
        }

        private void Button_KlikHer_Click(object sender, RoutedEventArgs e)
        {
            Menu_Indstillinger.IsChecked = true;
        }

        private void WarningDB_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Console.WriteLine(warningDB.ActualHeight);
            BindingOperations.GetBindingExpressionBase(gridForside, Grid.HeightProperty).UpdateTarget();
        }
    }
}
