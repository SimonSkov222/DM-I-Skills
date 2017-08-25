using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    [Serializable]
    public class TeamModel : ModelSettings
    {
        const int ERRNO_TIME_NULL = 1;
        const int ERRNO_CLASS_NULL = 2;
        const int ERRNO_TIME_FORMAT = 4;

        public const string ERROR_TIME_NULL = "";
        public const string ERROR_CLASS_NULL = "";


        public override int ErrNo {
            get
            {
                var numb = 0;
                Error = "";

                if ((Time == null || Time == "") || (Time != null && !Regex.IsMatch(Time, @"^\d\d:\d\d:\d\d$")))
                {
                    numb += ERRNO_TIME_NULL;
                }
                if (Class == null || Class == "")
                {
                    numb += ERRNO_CLASS_NULL;
                }

                //if (Time != null && !Regex.IsMatch(Time, @"\d\d:\d\d:\d\d"))
                //{

                //    numb += ERRNO_TIME_FORMAT;
                //}

                return numb;
            }
        }

        public int? ID { get; set; }
        public int SchoolID { get; set; }
        public int LocationID { get; set; }

        private string _Date;
        public string Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                NotifyPropertyChanged("CanUpload");
                NotifyPropertyChanged("ErrNo");
                NotifyPropertyOnAll?.Invoke();
            }
        }

        private string _Time;
        public string Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                NotifyPropertyChanged("CanUpload");
                NotifyPropertyChanged("ErrNo");
                NotifyPropertyOnAll?.Invoke();
            }
        }

        private string _Class;
        public string Class
        {
            get { return _Class; }
            set
            {
                _Class = value;
                NotifyPropertyChanged("CanUpload");
                NotifyPropertyChanged("ErrNo");
                NotifyPropertyOnAll?.Invoke();
            }
        }



        public override bool CanUpload
        {
            get
            {
                bool failed = false;
                
                //ErrNo = 0;
                Error = "";

                if ((Time == null || Time == "") || (Time != null && !Regex.IsMatch(Time, @"^\d\d:\d\d:\d\d$")))
                {
                    //ErrNo += ERRNO_TIME_NULL;
                    Error = ERROR_TIME_NULL;
                    failed = true;
                }
                if (Class == null || Class == "")
                {
                    //ErrNo += ERRNO_CLASS_NULL;
                    Error = ERROR_CLASS_NULL;
                    failed = true;
                }
                //if (Time != null && !Regex.IsMatch(Time, @"\d\d:\d\d:\d\d"))
                //{
                //    failed = true;
                //}


                NotifyPropertyChanged("ErrNo");

                return !failed;
            }
        }


        protected override bool OnUpload()
        {

            if (Date == null || Date == "")
            {
                Date = DateTime.Now.ToShortDateString();
            }
            var myDB = Scripts.Database.GetDB();
            if (ID != null)
            {
                return false;
            }
            else
            {
                ID = Convert.ToInt32(myDB.Insert("Teams", 
                    new string[] { "Class", "SchoolID", "LocationID", "Date", "Time"}, 
                    new object[] { Class, SchoolID, LocationID, Date, Time }, true));
            }
            myDB.Disconnect();

            return true;


            //return true;


        }

        public static ObservableCollection<TeamModel> GetAll()
        {
            var myDB = Scripts.Database.GetDB();

            var rows = myDB.GetRows("Teams", new string[] { "ID", "Class", "SchoolID", "LocationID", "Date", "Time" });
            myDB.Disconnect();

            var values = new ObservableCollection<TeamModel>();

            foreach (var i in rows)
            {
                var team = new TeamModel();
                team.ID = Convert.ToInt32(i[0]);
                team.Class = i[1].ToString();
                team.SchoolID = Convert.ToInt32(i[2]);
                team.LocationID = Convert.ToInt32(i[3]);
                team.Date = i[4].ToString();
                team.Time = i[5].ToString();
                values.Add(team);
            }
            return values;

        }

        public static ObservableCollection<TeamModel> GetTeamsByLocation(LocationModel location)
        {
            var values = new ObservableCollection<TeamModel>();
            if (location == null)
            {
                return values;
            }
            var myDB = Scripts.Database.GetDB();
            var rows = myDB.GetRows("Teams", new string[] { "ID", "Class", "SchoolID", "LocationID", "Date", "Time" },"WHERE `LocationID` = {0}", location.ID);
            myDB.Disconnect();

            if (rows != null)
            {
                foreach (var i in rows)
                {
                    var team = new TeamModel();
                    team.ID = Convert.ToInt32(i[0]);
                    team.Class = i[1].ToString();
                    team.SchoolID = Convert.ToInt32(i[2]);
                    team.LocationID = Convert.ToInt32(i[3]);
                    team.Date = i[4].ToString();
                    team.Time = i[5].ToString();
                    values.Add(team);
                }
            }
            return values;
        }

        
    }
}
