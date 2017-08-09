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


        public SchoolModel      School      { get; set; }
        public LocationModel    Location    { get; set; }
        public TeamModel        Team        { get; set; }
        public ObservableCollection<PersonModel> Persons    { get; set; }

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

            Persons.CollectionChanged += (o, e) => {

                Console.WriteLine("New Item");
                foreach (var item in e.NewItems)
                {
                    Console.WriteLine("-->");
                    ((PersonModel)item).NotifyPropertyOnAll += () => NotifyPropertyChanged("CanUpload");
                }

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
                Console.WriteLine("HasData");


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

                return false;
            }
        }

        public override bool CanUpload {
            get
            {
                Console.WriteLine("CanUpload");
                ErrNo = 0;

                if (!School.CanUpload)
                {
                    ErrNo += ERRNO_SCHOOL_NAME_NULL;
                }
                if (!Location.CanUpload)
                {
                    ErrNo += ERRNO_LOCATION_NAME_NULL;
                }
                if (!Team.CanUpload)
                {
                    ErrNo += ERRNO_TEAM_TIME_NULL;
                }

                if (Persons.Count == 0)
                {
                    ErrNo += ERRNO_PERSON_ZERO;
                }
                else if (Persons.Count(p => !p.CanUpload) > 0)
                {
                    ErrNo += ERRNO_SCHOOL_NAME_NULL;
                }

                NotifyPropertyChanged("ErrNo");
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

            return true;
        }


        public static ObservableCollection<TableModelN> GetTables() { return null; }
    }
}
