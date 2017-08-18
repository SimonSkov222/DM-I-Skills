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
        public string Debug { get; set; }

        public event Action OnConnection;
        public event Action<bool> OnDisconnection;

        public event Action OnUpload;

        public bool HasConnection { get { return IsServer || IsClient; } }
        public bool IsServer
        {
            get { return _IsServer; }
            set
            {
                _IsServer = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("HasConnection");
            }
        }
        public bool IsClient
        {
            get { return _IsClient; }
            set
            {
                _IsClient = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("HasConnection");
            }
        }
        private bool _IsServer = false;
        private bool _IsClient = false;

        public Scripts.Client Client;
        public Scripts.Server Server;



        public string FileNameLocalDB { get { return System.IO.Directory.GetCurrentDirectory() + @"\DB942.sqlite"; } }

        public string _FileNameDB = System.IO.Directory.GetCurrentDirectory() + @"\DB94.sqlite";
        public string FileNameDB
        {
            get { return _FileNameDB; }
            set
            {
                _FileNameDB = value;
                NotifyPropertyChanged();
            }
        }
        public string PrefixDB = "DM_Test5_";

        private int _TableCnt = 3;
        private int _OverTimeMin = 3;

        public int TableCnt { get { return _TableCnt; } set { _TableCnt = value; } }
        public int OverTimeMin { get { return _OverTimeMin; } set { _OverTimeMin = value; } }

        public LocationModel Location { get { return _Location; } set { _Location = value; NotifyPropertyChanged(); } }
        private LocationModel _Location;
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
            OnUpload?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
