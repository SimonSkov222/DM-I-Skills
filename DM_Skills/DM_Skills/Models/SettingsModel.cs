using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DM_Skills.Models
{
    class SettingsModel
    {
        public string Debug { get; set; }


        public bool IsServer { get; set; }
        public bool IsClient { get; set; }

        public Scripts.Client Client;
        public Scripts.Server Server;


        public static string FileNameLocalDB = System.IO.Directory.GetCurrentDirectory() + @"\DB942.sqlite";
        public static string FileNameDB = System.IO.Directory.GetCurrentDirectory() + @"\DB94.sqlite";
        public static string PrefixDB = "DM_Test5_";

        private int _TableCnt = 3;
        private int _OverTimeMin = 3;

        public int TableCnt { get { return _TableCnt; } set { _TableCnt = value; } }
        public int OverTimeMin { get { return _OverTimeMin; } set { _OverTimeMin = value; } }

        public LocationModel Location { get; set; }
        public ObservableCollection<LocationModel> AllLocations {
            get
            {

                Console.WriteLine("TODO: Settings.Locations");
                return new ObservableCollection<Models.LocationModel>();
                return LocationModel.GetAll();
            }
        }


        public ObservableCollection<SchoolModel> AllSchools { get {

                Console.WriteLine("TODO: Settings.AllSchools");
                return new ObservableCollection<SchoolModel>();
                return SchoolModel.GetAll();
            }
        }

    }
}
