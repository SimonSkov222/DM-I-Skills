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

namespace DM_Skills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateDatabase();
        }

        public void CreateDatabase()
        {
            string conStr = "Data Source=DatabaseSkillsDM.db;Version=3;";
            Database.Connect(conStr, "DM_");

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

        private void TimerControl_OnLap(TimeSpan obj)
        {
            LapList.Add(obj);
        }

        private void TimerControl_OnReset()
        {
            LapList.Reset();
        }
    }
}
