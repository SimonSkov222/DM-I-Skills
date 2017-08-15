using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    [Serializable]
    public class TableModelN : ModelSettings
    {
        const int ERRNO_SCHOOL_NAME_NULL        = 1;
        const int ERRNO_LOCATION_NAME_NULL      = 2;
        const int ERRNO_TEAM_TIME_NULL          = 4;
        const int ERRNO_PERSON_ZERO             = 8;
        const int ERRNO_PERSON_HAS_NAME_NULL    = 16;


        const string ERROR_SCHOOL_NAME_NULL         = SchoolModel.ERROR_NAME_NULL;
        const string ERROR_LOCATION_NAME_NULL       = LocationModel.ERROR_NAME_NULL;
        const string ERROR_TEAM_TIME_NULL           = TeamModel.ERROR_TIME_NULL;
        const string ERROR_PERSON_ZERO              = "";
        const string ERROR_PERSONS_HAS_NAME_NULL    = "";

        private bool _FailedUpload = false;

        public SchoolModel      School      { get; set; }
        public LocationModel    Location    { get; set; }
        public TeamModel        Team        { get; set; }
        public ObservableCollection<PersonModel> Persons    { get; set; }
        public bool FailedUpload {
            get { return _FailedUpload; }
            set
            {
                _FailedUpload = value;
                NotifyPropertyChanged("FailedUpload");
            }
        }

        public override int ErrNo
        {
            get
            {
                var numb = 0;

                if (!School.CanUpload)
                {
                    numb += ERRNO_SCHOOL_NAME_NULL;
                }
                if (!Location.CanUpload)
                {
                    numb += ERRNO_LOCATION_NAME_NULL;
                }
                if (!Team.CanUpload)
                {
                    numb += ERRNO_TEAM_TIME_NULL;
                }

                if (Persons.Count == 0 || Persons.Count(p => !p.CanUpload) > 0)
                {
                    numb += ERRNO_PERSON_ZERO;
                }

                return numb;
            }
        }


        public bool ErrPerson {
            get
            {

                return false;
            }
        }

        public TableModelN()
        {
            School      = new SchoolModel();
            Location    = new LocationModel();
            Team        = new TeamModel();
            Persons     = new ObservableCollection<PersonModel>();
            FailedUpload = false;

            Persons.CollectionChanged += (o, e) => {
                
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        ((PersonModel)item).NotifyPropertyOnAll += () =>
                        {
                            NotifyPropertyChanged("ErrNo");
                            NotifyPropertyChanged("HasData");
                        };
                    }
                }


                NotifyPropertyChanged("ErrNo");
                NotifyPropertyChanged("HasData");
            };
            School.NotifyPropertyOnAll += () => NotifyPropertyChanged("HasData");
            Team.NotifyPropertyOnAll += () => NotifyPropertyChanged("HasData");



            School.CallbackUpload   += o => Team.SchoolID   = (o as SchoolModel).ID;
            Location.CallbackUpload += o => Team.LocationID = (o as LocationModel).ID;
            Team.CallbackUpload += o =>
            {
                foreach (var p in Persons)
                    p.TeamID = (o as TeamModel).ID ?? 0;
            };
        }

        public bool HasData {
            get
            {
                if (School.Name != null && School.Name != "")
                {
                    return true;
                }
                if (Location.Name != null && Location.Name != "")
                {
                    return true;
                }
                if (Team.Class != null && Team.Class != "")
                {
                    return true;
                }
                if (Team.Time != null && Team.Time != "")
                {
                    return true;
                }
                if (Persons.Count > 0)
                {
                    return true;
                }
                //if(Persons.Count(p => p.Name == null || p.Name == "") > 0)

                return false;
            }
        }

        public override bool CanUpload {
            get
            {
                return ErrNo == 0;
            }
        }



        protected override bool OnUpload()
        {

            School.Upload();
            Location.Upload();
            Team.Upload();

            foreach (var p in Persons)
            {
                p.Upload();
            }


            FailedUpload = false;
            return true;
        }


        public static ObservableCollection<TableModelN> GetTables(string order, string schoolName, string personName, LocationModel location, string from, string to)
        {
            var result = new ObservableCollection<TableModelN>();
            var db = Scripts.Database.GetDB();

            string table_team = db.GetTableName("Teams");
            string table_schools = db.GetTableName("Schools");
            string table_locations = db.GetTableName("Locations");

            string cmd = string.Format("SELECT \n" +
            "´Team´.´ID´, ´Team´.´Class´, ´Team´.´Time´, ´Team´.´Date´, ´School´.´ID´, ´School´.´Name´, ´Location´.´ID´, ´Location´.´Name´ \n" +
             "FROM ´{0}´ AS ´Team´\n" +
             "INNER JOIN ´{1}´ AS ´School´ ON ´Team´.´SchoolID´ = ´School´.´ID´ \n" +
             "INNER JOIN ´{2}´ AS ´Location´ ON ´Team´.´LocationID´ = ´Location´.´ID´\n", table_team, table_schools, table_locations);

            List<string> where = new List<string>();



            if (personName != "")
            {
                personName = db.EscapeString(personName);
                db.UseDistinct = true;
                var persons = db.GetRows("Persons", "TeamID", "WHERE ´Name´ LIKE '%{0}%'", personName);
                if (persons != null)
                {
                    List<int> id = new List<int>();
                    foreach (var item in persons)
                    {
                        id.Add(Convert.ToInt32(item[0]));
                    }
                    if (id.Count > 0)
                    {
                        string cmdID = string.Join(", ", id.ToArray());
                        where.Add("´Team´.´ID´ IN (" + cmdID + ")");
                    }
                }
            }
            if (schoolName != "")
            {
                schoolName = db.EscapeString(schoolName);
                where.Add("´School´.´Name´ LIKE '%" + schoolName + "%'");
            }
            if (location != null && location.Name != "")
            {
                location.Name = db.EscapeString(location.Name);
                where.Add("´Location´.´Name´ LIKE '%" + location.Name + "%'");
            }
            //if (from != null && from != "")
            //{
            //    where.Add("[Team].[Date] >= '" + from + "'");
            //}
            //if (to != null && to != "")
            //{
            //    where.Add("[Team].[Date] <= '" + to + "'");
            //}
            if (where.Count > 0)
            {
                string cmdWhere = string.Join(" AND ", where.ToArray());
                cmd += " WHERE " + cmdWhere;
            }
            cmd += ";";

            var dataTeam = db.ExecuteQuery(cmd);

            if (dataTeam != null && dataTeam.Count > 0)
            {
                List<int> teamIDs = new List<int>();

                //"[Team].[ID], [Team].[Class], [Team].[Time], [Team].[Date], [School].[ID], [School].[Name], [Location].[ID], [Location].[Name] \n"

                foreach (var item in dataTeam)
                {
                    var model = new TableModelN();
                    teamIDs.Add(Convert.ToInt32(item[0]));
                    model.Team.ID = Convert.ToInt32(item[0]);
                    model.Team.Class = Convert.ToString(item[1]);
                    model.Team.Time = Convert.ToString(item[2]);
                    model.Team.Date = Convert.ToString(item[3]);
                    model.School.ID = Convert.ToInt32(item[4]);
                    model.School.Name = Convert.ToString(item[5]);
                    model.Location.ID = Convert.ToInt32(item[6]);
                    model.Location.Name = Convert.ToString(item[7]);

                    result.Add(model);
                }

                if (teamIDs.Count > 0)
                {
                    string cmdTeamIDs = string.Join(", ", teamIDs);

                    var persons = db.GetRows("Persons", new string[] { "ID", "Name", "TeamID" }, "WHERE ´TeamID´ IN ({0})", cmdTeamIDs);

                    if (persons != null)
                    {
                        foreach (var item in persons)
                        {
                            var model = new PersonModel();
                            model.ID = Convert.ToInt32(item[0]);
                            model.Name = Convert.ToString(item[1]);
                            model.TeamID = Convert.ToInt32(item[2]);

                            var team = result.Where(o => o.Team.ID == model.TeamID).ToArray();

                            if (team.Length > 0)
                            {
                                team[0].Persons.Add(model);
                            }
                        }
                    }
                }
            }

            Console.WriteLine(cmd);

            return result;
        }
    }
}
