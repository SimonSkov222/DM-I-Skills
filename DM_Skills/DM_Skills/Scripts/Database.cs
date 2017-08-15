using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Scripts
{
    class Database
    {
        private static Models.SettingsModel Settings;


        private static IDatabase db;
        private static string Host = "192.168.4.221";
        private static string User = "dm";
        private static string Pass = "DMiSKILLS2017";
        private static string DatabaseName = "DM_i_Skills";

        public static IDatabase GetDB()
        {
            if (Settings == null)
            {
                Settings = Application.Current.FindResource("Settings") as Models.SettingsModel;
            }

            if (db == null)
            {
                db = new MSSQL();
            }

            if (!db.IsConnected) {
                string connString = string.Format("Server={0};Database={1};User Id={2};Password={3}", Host, DatabaseName, User, Pass);
                db.Connect(connString, "DM_Test2_");
            }

            return db;
        }

        public static void CreateDatabase() {
            var myDB = GetDB();
            
            if (!myDB.Exist("Schools"))
            {
                myDB.Create("Schools",
                    new MSColumns { name = "ID", type = MSColumns.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new MSColumns { name = "Name", type = MSColumns.TYPE_VARCHAR, isNotNull = true }
                );
            }

            if (!myDB.Exist("Locations"))
            {
                myDB.Create("Locations",
                    new MSColumns { name = "ID", type = MSColumns.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new MSColumns { name = "Name", type = MSColumns.TYPE_VARCHAR, isNotNull = true }
                );

                myDB.Insert("Locations", "Name", "Ballerup");
                myDB.Insert("Locations", "Name", "Hvidovre");
            }

            if (!myDB.Exist("Teams"))
            {
                myDB.Create("Teams",
                    new MSColumns { name = "ID", type = MSColumns.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new MSColumns { name = "SchoolID", type = MSColumns.TYPE_INT, isNotNull = true, foreignKeyReferences = "Schools(ID)" },
                    new MSColumns { name = "LocationID", type = MSColumns.TYPE_INT, isNotNull = true, foreignKeyReferences = "Locations(ID)" },
                    new MSColumns { name = "Class", type = MSColumns.TYPE_VARCHAR, isNotNull = true },
                    new MSColumns { name = "Time", type = MSColumns.TYPE_VARCHAR, isNotNull = true },
                    new MSColumns { name = "Date", type = MSColumns.TYPE_VARCHAR, isNotNull = true }
                );
            }

            if (!myDB.Exist("Persons"))
            {
                myDB.Create("Persons",
                    new MSColumns { name = "ID", type = MSColumns.TYPE_INT, isPrimaryKey = true, isAutoIncrement = true },
                    new MSColumns { name = "TeamID", type = MSColumns.TYPE_INT, isNotNull = true, foreignKeyReferences = "Teams(ID)" },
                    new MSColumns { name = "Name", type = MSColumns.TYPE_VARCHAR, isNotNull = true }
                );
            }


            myDB.Disconnect();
        }

    }
}
