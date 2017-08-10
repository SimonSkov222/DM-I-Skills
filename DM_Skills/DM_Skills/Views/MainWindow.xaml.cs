using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SQLite_DB_LIB;                    //Database
using System.ComponentModel;            //INotifyPropertyChanged
using System.Runtime.CompilerServices;  //CallerMemberName for PropertyChanged

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
            InitializeComponent();



            //Database.Connect("Data Source=DatabaseSkillsDM.db;Version=3;", "DM_");
            //CreateDatabase();

            Scripts.Database.CreateDatabase();

            //Loaded += (o, e) =>
            //{
            //    Console.WriteLine(view_forside.stopwatch.DisplayTime);
            //    var win = new Views.Projektor(view_forside.stopwatch);
            //    win.Timer = view_forside.stopwatch;
            //    win.Show();

            Loaded += (o, e) => { Menu_Forside.IsChecked = false; Menu_Forside.IsChecked = true; };

            Loaded += (o, e) => {
                //projek = new Views.Projektor(view_forside.stopwatch, this);
                //projek.Closed += (oo, ee) => { Menu_Projektor.IsChecked = false; };
            };

            //};
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
            //debug_plc.Persons.Add(new Models.PersonModel());
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
    }
}
