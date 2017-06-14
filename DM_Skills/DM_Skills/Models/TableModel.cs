using System;
using SQLite_DB_LIB;

namespace DM_Skills.Models
{
    /// <summary>
    /// Denne Klasse styre regler og hvordan
    /// man kan uploade til databasen
    /// </summary>
    public class TableModel
    {

        public string School { get; set; }
        public string Persons { get; set; }
        public string Location { get; set; }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Class { get; set; }
        public int Team { get; set; }
        public string Time { get; set; }
        

        /// <summary>
        /// Tjek om klassen har data
        /// </summary>
        public bool HasData()
        {
            return (School != null && School != "") 
                || (Persons != null && Persons != "")
                || (Class != null && Class != "")
                || Time != null && Time != new TimeSpan().ToString();
        }

        /// <summary>
        /// Tjek om vi må/kan uploade til database
        /// </summary>
        public bool CanUpload()
        {
            return (School != null && School != "")
                && (Persons != null && Persons != "")
                && (Class != null && Class != "")
                && Time != null && Time != new TimeSpan().ToString();
        }

        /// <summary>
        /// Upload dataen til databasen
        /// </summary>
        public void Uplaod()
        {
            //Tjek om skolen findes i database og hent dens id bagefter
            if (!Database.Exist("Schools","Name",School))
                Database.Insert("Schools","Name",School);

            var SchoolId = Database.GetRow<int>("Schools", new string[] { "ID" }, string.Format("WHERE `Name` = '{0}'", School));

            //Tjek om lokationen findes i database og hent dens id bagefter
            if (!Database.Exist("Locations", "Name", Location))
                Database.Insert("Locations", "Name", Location);
            var LocationId = Database.GetRow<int>("Locations", new string[] { "ID" }, string.Format("WHERE `Name` = '{0}'", Location));
            
            //Gem holdet i databasen og hent dens id
            var TeamId = Database.Insert("Teams", new string[] { "SchoolID", "LocationID", "Class", "Number", "Time", "Date" }, new object[] { SchoolId[0], LocationId[0], Class, Team, Time, Date.ToShortDateString() });

            //Gem deltagerne til holdet
            string[] Names = Persons.Replace(", ", ",").Split(',');
            foreach (var item in Names)
                Database.Insert("Persons", new string[] { "TeamID", "Name" }, new object[] { TeamId, item});

        }



    }
}
