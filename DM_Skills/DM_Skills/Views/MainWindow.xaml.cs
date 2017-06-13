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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimonSkov.SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DM_Skills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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
                NotifyPropertyChanged("Round");
            }
        }

        /****************************
        * 
        *      Interface: INotifyPropertyChanged
        * 
        ***************************/
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainWindow()
        {
            Database.Connect("Data Source=DatabaseSkillsDM.db;Version=3;", "DM_");
            

            InitializeComponent();
            CreateDatabase();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NotifyPropertyChanged("SchoolAutoComplete");
            UpdateTableTeam();
            UpdateSchoolList();
        }

        public void CreateDatabase()
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

        private void UpdateTableTeam()
        {
            cTable1.TeamID = (Round -1) * 10 + 1;
            cTable2.TeamID = (Round -1) * 10 + 2;
            cTable3.TeamID = (Round -1) * 10 + 3;
            cTable4.TeamID = (Round -1) * 10 + 4;
            cTable5.TeamID = (Round -1) * 10 + 5;
        }

        private void UpdateSchoolList()
        {
            cTable1.schoolList.Children.Clear();
            cTable2.schoolList.Children.Clear();
            cTable3.schoolList.Children.Clear();
            cTable4.schoolList.Children.Clear();
            cTable5.schoolList.Children.Clear();

            foreach (var item in SchoolAutoComplete)
            {
                cTable1.schoolList.Children.Add(item);
                cTable2.schoolList.Children.Add(item);
                cTable3.schoolList.Children.Add(item);
                cTable4.schoolList.Children.Add(item);
                cTable5.schoolList.Children.Add(item);
            }
        }

        private void TimerControl_OnLap(TimeSpan obj)
        {
            LapList.Add(obj);
        }

        private void TimerControl_OnReset()
        {
            LapList.Reset();
        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            UserControl[] values =
            {
                cTable1,
                cTable2,
                cTable3,
                cTable4,
                cTable5
            };

            bool canUpload = true;

            foreach (var table in values)
            {
                var model = table.DataContext as Models.TableModel;
                if (model.HasData() && !model.CanUpload())
                {
                    table.BorderThickness = new Thickness(2);
                    table.BorderBrush = Brushes.Red;
                    canUpload = false;
                }
                else
                {
                    table.BorderThickness = new Thickness(0);
                }
            }

            if (canUpload)
            {
                foreach (var table in values)
                {
                    var model = table.DataContext as Models.TableModel;
                    if (model.HasData())
                        model.Uplaod();
                }

                MessageBox.Show("Er nu uploadet til databasen.");
                Round++;
                UpdateTableTeam();
                UpdateSchoolList();
                Button_Reset_Click(null, null);
            }
            else
            {
                MessageBox.Show("Kan ikke uploade. Ret fejlne.");
            }
            
            NotifyPropertyChanged("SchoolAutoComplete");
        }

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
