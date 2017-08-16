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
        private static IDatabase localDB;
        private static string Host = "192.168.4.221";
        private static string User = "dm";
        private static string Pass = "DMiSKILLS2017";
        private static string DatabaseName = "DM_i_Skills";


        public static IDatabase GetLocalDB()
        {
            if (Settings == null)
            {
                Settings = Application.Current.FindResource("Settings") as Models.SettingsModel;
            }

            if (localDB == null)
            {
                localDB = new SQLite();
            }

            if (!localDB.IsConnected)
            {
                string connString = string.Format("Data Source={0};Version=3;", Models.SettingsModel.FileNameLocalDB);
                db.Connect(connString, Models.SettingsModel.PrefixDB);

            }

            return localDB;
        }
        
        public static IDatabase GetDB()
        {
            if (Settings == null)
            {
                Settings = Application.Current.FindResource("Settings") as Models.SettingsModel;
            }

            if (db == null)
            {
                db = new SQLite(true);
            }

            if (!db.IsConnected) {
                //string connString = string.Format("Server={0};Database={1};User Id={2};Password={3}", Host, DatabaseName, User, Pass);
                string connString = string.Format("Data Source={0};Version=3;", Models.SettingsModel.FileNameDB);
                db.Connect(connString, Models.SettingsModel.PrefixDB);
            }

            return db;
        }

        public static void CreateLocalDatabase() {
            var myDB = GetLocalDB();
            if (!myDB.Exist("Settings"))
            {
                myDB.Create("Settings",
                    new Column() { Name = "Name", Type = ColumnTypes.String, IsPrimaryKey=true},
                    new Column() { Name = "Value", Type = ColumnTypes.String, IsNotNull=true }
                );
            }
            myDB.Disconnect();
        }

        public static void CreateDatabase() {
            var myDB = GetDB();
            
            if (!myDB.Exist("Schools"))
            {
                myDB.Create("Schools",
                    new Column { Name = "ID", Type = ColumnTypes.Int, IsPrimaryKey = true, IsAutoIncrement = true },
                    new Column { Name = "Name", Type = ColumnTypes.String, IsNotNull = true }
                );
            }

            if (!myDB.Exist("Locations"))
            {
                myDB.Create("Locations",
                    new Column { Name = "ID", Type = ColumnTypes.Int, IsPrimaryKey = true, IsAutoIncrement = true },
                    new Column { Name = "Name", Type = ColumnTypes.String, IsNotNull = true }
                );

                myDB.Insert("Locations", "Name", "Ballerup");
                myDB.Insert("Locations", "Name", "Hvidovre");
            }

            if (!myDB.Exist("Teams"))
            {
                myDB.Create("Teams",
                    new Column { Name = "ID", Type = ColumnTypes.Int, IsPrimaryKey = true, IsAutoIncrement = true },
                    new Column { Name = "SchoolID", Type = ColumnTypes.Int, IsNotNull = true, ForeignKeyReferences = "Schools(ID)" },
                    new Column { Name = "LocationID", Type = ColumnTypes.Int, IsNotNull = true, ForeignKeyReferences = "Locations(ID)" },
                    new Column { Name = "Class", Type = ColumnTypes.String, IsNotNull = true },
                    new Column { Name = "Time", Type = ColumnTypes.String, IsNotNull = true },
                    new Column { Name = "Date", Type = ColumnTypes.String, IsNotNull = true }
                );
            }

            if (!myDB.Exist("Persons"))
            {
                myDB.Create("Persons",
                    new Column { Name = "ID", Type = ColumnTypes.Int, IsPrimaryKey = true, IsAutoIncrement = true },
                    new Column { Name = "TeamID", Type = ColumnTypes.Int, IsNotNull = true, ForeignKeyReferences = "Teams(ID)" },
                    new Column { Name = "Name", Type = ColumnTypes.String, IsNotNull = true }
                );
            }


            myDB.Disconnect();
        }

    }
}
