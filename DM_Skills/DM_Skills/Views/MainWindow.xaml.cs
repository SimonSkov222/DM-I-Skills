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
            Scripts.Database.CreateDatabase();
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

        
        
        /// <summary>
        /// Opretter databasen.
        /// Vi tjekker om en table findes i database 
        /// og hvis den ikke gør det vil vi oprette den
        /// </summary>
        private void CreateDatabase()
        {
            if (!Database.Exist("Schools"))
            {
                Database.Create("Schools",
                    new Column { name = "ID", type = Column.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new Column { name = "Name", type = Column.TYPE_STRING, isNotNull = true }
                );
            }

            if (!Database.Exist("Locations"))
            {
                Database.Create("Locations",
                    new Column { name = "ID", type = Column.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new Column { name = "Name", type = Column.TYPE_STRING, isNotNull = true }
                );

                Database.Insert("Locations", "Name", "Ballerup");
                Database.Insert("Locations", "Name", "Hvidovre");
            }

            if (!Database.Exist("Teams"))
            {
                Database.Create("Teams",
                    new Column { name = "ID", type = Column.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new Column { name = "SchoolID", type = Column.TYPE_INT, isNotNull = true, foreignKeyReferences = "Schools(ID)" },
                    new Column { name = "LocationID", type = Column.TYPE_INT, isNotNull = true, foreignKeyReferences = "Locations(ID)" },
                    new Column { name = "Class", type = Column.TYPE_STRING, isNotNull = true },
                    new Column { name = "Number", type = Column.TYPE_STRING, isNotNull = true },
                    new Column { name = "Time", type = Column.TYPE_STRING, isNotNull = true },
                    new Column { name = "Date", type = Column.TYPE_STRING, isNotNull = true }
                );
            }

            if (!Database.Exist("Persons"))
            {
                Database.Create("Persons",
                    new Column { name = "ID", type = Column.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new Column { name = "TeamID", type = Column.TYPE_INT, isNotNull = true, foreignKeyReferences = "Teams(ID)" },
                    new Column { name = "Name", type = Column.TYPE_STRING, isNotNull = true }
                );
            }


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
            var server = new Scripts.Server();
            server.Connect(7788);
        }
        Scripts.Client Client;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Client = new Scripts.Client();
            Client.Connect("127.0.0.1", 7788);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Client.Send(Scripts.PacketType.GetSchools, (o) =>
            {
                Console.WriteLine("Got Reply");
                Console.WriteLine(o.GetType());
            });
            
        }
    }
}
