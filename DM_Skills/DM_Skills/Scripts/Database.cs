using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class Database
    {

        private static IDatabase db;
        private static string Host = "192.168.4.221";
        private static string User = "dm";
        private static string Pass = "DMiSKILLS2017";
        private static string DatabaseName = "DM_i_Skills";


        public static IDatabase GetDB()
        {
            if (db == null)
            {
                db = new SQLite();
            }

            if (!db.IsConnected) {
                //string connString = string.Format("Server={0};Database={1};User Id={2};Password={3}", Host, DatabaseName, User, Pass);
                string connString = string.Format("Data Source={0};Version=3;", Models.SettingsModel.FileNameDB);
                db.Connect(connString, "DM_Test2_");
            }

            return db;
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
