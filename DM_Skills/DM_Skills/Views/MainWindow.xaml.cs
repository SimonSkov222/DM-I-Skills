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
        public List<string> SchoolAutoComplete { get; set;}

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
            SchoolAutoComplete = new List<string>();
            Database.Connect("Data Source=DatabaseSkillsDM.db;Version=3;", "DM_");
            

            InitializeComponent();
            CreateDatabase();

            var data = Database.GetRows<int>("Schools", new string[] { "Name" });
            if (data != null)
            {
                SchoolAutoComplete.Clear();
                foreach (var item in data)
                {
                    SchoolAutoComplete.Add((string)item[0]);
                }
            }
            NotifyPropertyChanged("SchoolAutoComplete");
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

        private void TimerControl_OnLap(TimeSpan obj)
        {
            LapList.Add(obj);
        }

        private void TimerControl_OnReset()
        {
            LapList.Reset();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           var table1 = new Models.TableModel();
            table1.Location = "Ballerup";
            table1.School = "Min Skole";
            table1.Class = "4a";
            table1.Persons = "Kim,Simon";
            table1.Time = "01:00:11";
            table1.Date = DateTime.Now;

            //Console.WriteLine(table1.HasData());
            //Console.WriteLine(table1.CanUpload());
            table1.Uplaod();

            var data = Database.GetRows<int>("Schools", new string[] { "Name" });
            if (data != null)
            {
                SchoolAutoComplete.Clear();
                foreach (var item in data)
                {
                    SchoolAutoComplete.Add((string)item[0]);
                }
            }
            NotifyPropertyChanged("SchoolAutoComplete");
        }
    }
}
