using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
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


        public static ObservableCollection<TableModelN> GetTables(string order, string schoolName, string personName, LocationModel location, DateTime from, DateTime to)
        {
            //SELECT [] FROM [Person]

            //tabTeam
            //SELECT [] FROM [tabTeam] AS [T]
            // INNER JOIN [] as [TT] ON [T].[COLUMN] = [TT].[COLUMN];

            return null;
        }
    }
}
