using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SimonSkov.SQLite;

namespace DM_Skills.Models
{
    public class TableModel : Control
    {

        public string School { get; set; }
        public string Persons { get; set; }
        public string Location { get; set; }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Class { get; set; }
        public int Team { get; set; }
        public string Time { get; set; }
        


        public bool HasData()
        {
            return (School != null && School != "") 
                || (Persons != null && Persons != "")
                || (Class != null && Class != "")
                || Time != null && Time != new TimeSpan().ToString();
        }
        public bool CanUpload()
        {
            return (School != null && School != "")
                && (Persons != null && Persons != "")
                && (Class != null && Class != "")
                && Time != null && Time != new TimeSpan().ToString();
        }
        public void Uplaod()
        {
            if (!Database.Exist("Schools","Name",School))
                Database.Insert("Schools","Name",School);

            var SchoolId = Database.GetRow<int>("Schools", new string[] { "ID" }, string.Format("WHERE `Name` = '{0}'", School));

            if (!Database.Exist("Locations", "Name", Location))
                Database.Insert("Locations", "Name", Location);
            var LocationId = Database.GetRow<int>("Locations", new string[] { "ID" }, string.Format("WHERE `Name` = '{0}'", Location));
            
            var TeamId = Database.Insert("Teams", new string[] { "SchoolID", "LocationID", "Class", "Number", "Time", "Date" }, new object[] { SchoolId[0], LocationId[0], Class, Team, Time, Date.ToShortDateString() });

            string[] Names = Persons.Replace(", ", ",").Split(',');

            foreach (var item in Names)
            {
                Database.Insert("Persons", new string[] { "TeamID", "Name" }, new object[] { TeamId, item});
            }

        }



    }
}
