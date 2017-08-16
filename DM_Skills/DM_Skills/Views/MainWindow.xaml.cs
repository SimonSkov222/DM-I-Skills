using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SQLite_DB_LIB;                    //Database
using System.ComponentModel;            //INotifyPropertyChanged
using System.Runtime.CompilerServices;  //CallerMemberName for PropertyChanged
using System.Windows.Documents;

namespace DM_Skills
{
    /// <summary>
    /// Klassen arver fra Window og bruger interface`et INotifyPropertyChanged
    /// Klassen styre det der skal ske i tidtagning fanen
    /// </summary>
    public partial class MainWindow : Window
    {

        public Views.Projektor projek;

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


            //Random r = new Random();
            //for (int i = 0; i < 10; i++)
            //{

            //    var d = new Models.TableModelN();
            //    d.Team.Class = "7z" + i;
            //    d.Team.Date = $"{r.Next(1, 25).ToString().PadLeft(2,'0')}-{r.Next(1, 11).ToString().PadLeft(2, '0')}-{r.Next(2015, 2018)}";
            //    d.Team.Time = $"{r.Next(10, 30)}:{r.Next(10, 59)}:{r.Next(10, 59)}";
            //    d.School.Name = $"Hersted {r.Next(100)} Skole";
            //    d.Location.Name = "ballerup";
            //    d.Persons.Add(new Models.PersonModel() { Name = "hej" });
            //    d.Persons.Add(new Models.PersonModel() { Name = "meh" });


            //    var t = new Models.TableModelN();
            //    t.Team.Class = "8c" + i;
            //    t.Team.Date = $"{r.Next(1,25).ToString().PadLeft(2, '0')}-{r.Next(1, 11).ToString().PadLeft(2, '0')}-{r.Next(2015, 2018)}";
            //    t.Team.Time = $"{r.Next(10,30)}:{r.Next(10,59)}:{r.Next(10,59)}";
            //    t.School.Name = $"Måeløv {r.Next(100)} Skole";
            //    t.Location.Name = "Hvidovre";
            //    t.Persons.Add(new Models.PersonModel() { Name = "karl" });
            //    t.Persons.Add(new Models.PersonModel() { Name = "jarl" });

            //    d.Upload();
            //    t.Upload();

            //    d = null;
            //    t = null;
            //}

            //Database.Connect("Data Source=DatabaseSkillsDM.db;Version=3;", "DM_");
            //CreateDatabase();



            //Loaded += (o, e) =>
            //{
            //    Console.WriteLine(view_forside.stopwatch.DisplayTime);
            //    var win = new Views.Projektor(view_forside.stopwatch);
            //    win.Timer = view_forside.stopwatch;
            //    win.Show();

            Loaded += (o, e) => { Menu_Forside.IsChecked = false; Menu_Forside.IsChecked = true; };

        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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


            var schools = Models.SchoolModel.GetAll();
            Models.SettingsModel.lab.Content += "Skoler:\n";
            foreach (var item in schools)
            {
                Models.SettingsModel.lab.Content += $"  ID: {item.ID}, Name: {item.Name}\n";
            }
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
    }
}
