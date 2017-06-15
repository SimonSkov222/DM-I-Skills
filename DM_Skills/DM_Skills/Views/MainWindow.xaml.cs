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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Vi søger i databasen efter alle skolerne 
        /// og laver det om til en list af strings
        /// </summary>
        public List<string> SchoolAutoComplete { get {
                List<string> result = new List<string>();
                if (Database.IsConnected)
                {
                    var data = Database.GetRows<int>("Schools", new string[] { "Name" });
                    if (data != null)
                        foreach (var item in data)
                            result.Add((string)item[0]);
                }
                return result;
            }
        }

        private int _Round = 1;
        public int Round {
            get { return _Round; }
            set {
                _Round = value;
                NotifyPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fortæller at en property har ændret sig
        /// </summary>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //Tjekker om PropertyChanged er sat og kalder den hvis den er
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Her connecter vi til databasen og opretter den
        /// vi tilføjet også en load event
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            
            Database.Connect("Data Source=DatabaseSkillsDM.db;Version=3;", "DM_");
            CreateDatabase();

            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Når vinduet er loaded vil vi opdatere hold nummere 
        /// og skole autocomplete liste for bordene
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTableTeam();
            UpdateSchoolList();
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

        /// <summary>
        /// Giver alle bordene det rigtige hold nummer alt efter runde
        /// (Vi har lavet denne function fordi vores
        /// data binding ikke ville gøre det for os)
        /// </summary>
        private void UpdateTableTeam()
        {
            cTable1.TeamID = (Round -1) * 10 + 1;
            cTable2.TeamID = (Round -1) * 10 + 2;
            cTable3.TeamID = (Round -1) * 10 + 3;
            cTable4.TeamID = (Round -1) * 10 + 4;
            cTable5.TeamID = (Round -1) * 10 + 5;
        }

        /// <summary>
        /// Opdatere autocomplete listen med skolerne for bordene
        /// (Vi har lavet denne function fordi vores
        /// data binding ikke ville gøre det for os)
        /// </summary>
        private void UpdateSchoolList()
        {
            //Nulstil bordene
            cTable1.schoolList.Children.Clear();
            cTable2.schoolList.Children.Clear();
            cTable3.schoolList.Children.Clear();
            cTable4.schoolList.Children.Clear();
            cTable5.schoolList.Children.Clear();

            //Tilføj skolerne
            foreach (var item in SchoolAutoComplete)
            {
                cTable1.schoolList.Children.Add(item);
                cTable2.schoolList.Children.Add(item);
                cTable3.schoolList.Children.Add(item);
                cTable4.schoolList.Children.Add(item);
                cTable5.schoolList.Children.Add(item);
            }
        }
        
        /// <summary>
        /// StopUr omgang klik.
        /// Tilføj omgang tiden til vores drag and drop list 
        /// </summary>
        private void TimerControl_OnLap(TimeSpan obj)
        {
            LapList.Add(obj);
        }

        /// <summary>
        /// StopUr Nulstil klik.
        /// Gør at vores drag and drop list også bliver resat
        /// </summary>
        private void TimerControl_OnReset()
        {
            LapList.Reset();
        }

        /// <summary>
        /// Indsend klik
        /// Her tjekker vi om bordene er udfyldt rigtigt
        /// og uploader dem hvis de er
        /// 
        /// Brugeren får også besked om man kunne
        /// uploade eller ej
        /// </summary>
        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            //Gør vi kan loop igennem bordene med et loop
            UserControl[] values =
            {
                cTable1,
                cTable2,
                cTable3,
                cTable4,
                cTable5
            };

            //Fortæller om vi kan oploade eller ej
            bool canUpload = true;

            foreach (var table in values)
            {
                //Hent model og tjek om vi overholder 
                //reglerne for at kunne uploade
                var model = table.DataContext as Models.TableModel;
                if (model.HasData() && !model.CanUpload())
                {
                    //Highlight det bord der er en fejl på
                    table.BorderThickness = new Thickness(2);
                    table.BorderBrush = Brushes.Red;
                    canUpload = false;
                }
                else
                {
                    //Fjern highlight
                    table.BorderThickness = new Thickness(0);
                }
            }


            //Tjek om må uploade
            if (canUpload)
            {
                //Start upload for de bordere der har data
                foreach (var table in values)
                {
                    var model = table.DataContext as Models.TableModel;
                    if (model.HasData())
                        model.Uplaod();
                }

                
                MessageBox.Show("Er nu uploadet til databasen.");

                //Gå til næste runde
                Round++;
                UpdateTableTeam();
                UpdateSchoolList();
                Button_Reset_Click(null, null);
            }
            else
            {
                MessageBox.Show("Kan ikke uploade. Ret fejlne.");
            }
        }


        /// <summary>
        /// Nulstiling af runde klik.
        /// </summary>
        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            cTable1.Reset();
            cTable2.Reset();
            cTable3.Reset();
            cTable4.Reset();
            cTable5.Reset();

            stopwatch.Reset();
            LapList.Reset();
        }
    }
}
