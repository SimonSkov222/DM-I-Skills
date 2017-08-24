using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace DM_Skills.Models
{
    class SettingsModel : INotifyPropertyChanged
    {


        public string Version { get { return "1.0"; } }
        public string Author { get { return "Kim Danborg & Simon Skov"; } }
        public string Copyright
        {
            get
            {
                int created = 2017;
                string value = $"Copyright {created}";
                if(DateTime.Now.Year > created)
                {
                    value += $" - {DateTime.Now.Year}";
                }
                return value;
            }
        }

        public string Debug { get; set; }

        public event Action OnSchoolsChanged;
        public event Action OnConnection;
        public event Action<bool> OnDisconnection;

        public event Action OnLocationChanged;

        public event Action OnUpload;

        public bool HasConnection { get { return IsServer || IsClient; } }
        public bool HasLostConnection { get { return !HasConnection && HasConnectionBefore; } }
        private bool HasConnectionBefore = false;

        public bool IsServer
        {
            get { return _IsServer; }
            set
            {
                _IsServer = value;
                if (value)
                {
                    HasConnectionBefore = true;
                }
                NotifyPropertyChanged();
                NotifyPropertyChanged("HasConnection");
                NotifyPropertyChanged("HasLostConnection");
            }
        }
        public bool IsClient
        {
            get { return _IsClient; }
            set
            {
                _IsClient = value;
                if (value)
                {
                    HasConnectionBefore = true;
                }
                NotifyPropertyChanged();
                NotifyPropertyChanged("HasConnection");
                NotifyPropertyChanged("HasLostConnection");
            }
        }
        private bool _IsServer = false;
        private bool _IsClient = false;

        public Scripts.Client Client;
        public Scripts.Server Server;

        public string FileNameLocalDB { get { return System.IO.Directory.GetCurrentDirectory() + @"\DMiSkillsLocalDB.sqlite"; } }

        public string FileNameDefaultDB = System.IO.Directory.GetCurrentDirectory() + @"\DMiSkillsDB.sqlite";

        private string _FileNameDB = null;
        public string FileNameDB
        {
            get
            {
                if (_FileNameDB == null)
                {
                    var db = Scripts.Database.GetLocalDB("Get FileNameDB ID:" + System.Threading.Thread.CurrentThread.Name);
                    _FileNameDB = db.GetRow("Settings", "Value", "WHERE `Name` = 'LocationDB'")[0].ToString();
                    db.Disconnect();
                }
                
                return _FileNameDB;
            }
            set
            {
                _FileNameDB = null;
                var db = Scripts.Database.GetLocalDB("Set FileNameDB");
                db.Update("Settings", "Value", value, (object)"LocationDB");
                db.Disconnect();
                NotifyPropertyChanged();
            }
        }
        public string PrefixDB = "DM_Test5_";

        private int _TableCnt = 3;
        private int _OverTimeMin = 3;

        public int TableCnt
        {
            get { return _TableCnt; }
            set
            {
                _TableCnt = value;
                NotifyPropertyChanged();
            }
        }
        public int OverTimeMin
        {
            get { return _OverTimeMin; }
            set
            {
                _OverTimeMin = value;
                NotifyPropertyChanged();
            }
        }
        private LocationModel _Location;
        public LocationModel Location
        {
            get { return _Location; }
            set
            {

                _Location = value;
                NotifyPropertyChanged(nameof(HasLocation));
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<LocationModel> _AllLocations = null;
        public ObservableCollection<LocationModel> AllLocations {
            get
            {
                if (_AllLocations == null)
                {
                    _AllLocations = LocationModel.GetAll();
                    _AllLocations.Insert(0, new Models.LocationModel() { ID = -1, Name = "Vælg lokation" });
                }
                return _AllLocations;
            }
        }

        private ObservableCollection<SchoolModel> _AllSchools = null;
        public ObservableCollection<SchoolModel> AllSchools
        {
            get
            {
                Console.WriteLine("Call All_Schools");
                if (_AllSchools == null)
                {
                    Console.WriteLine("Get Schools");
                    _AllSchools = SchoolModel.GetAll();
                }
                return _AllSchools;
            }
        }

        public void InvokeSchoolsChanged()
        {
            NotifyPropertyChanged(nameof(AllSchools));
            OnSchoolsChanged?.Invoke();
        }
        public void InvokeConnection()
        {
            OnConnection?.Invoke();
        }


        public void InvokeDisconnection(bool disconnectedByUser)
        {
            OnDisconnection?.Invoke(disconnectedByUser);
        }

        public void InvokeUpload()
        {
            Console.WriteLine("InvokeUpload");
            OnUpload?.Invoke();
        }

        public void InvokeLocationChanged()
        {
            //NotifyPropertyChanged();
            OnLocationChanged?.Invoke();
        }
        
        public bool HasLocation
        {
            get
            {
                return _Location != null && _Location.ID != -1;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(AllLocations): _AllLocations = null; break;
                case nameof(AllSchools): _AllSchools = null; break;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




    }
}
