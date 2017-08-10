using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    class SettingsModel
    {
        private int _TableCnt = 3;
        private int _OverTimeMin = 3;

        public int TableCnt { get { return _TableCnt; } set { _TableCnt = value; } }
        public int OverTimeMin { get { return _OverTimeMin; } set { _OverTimeMin = value; } }

        public LocationModel Location { get; set; }
        public ObservableCollection<LocationModel> AllLocations { get { return LocationModel.GetAll(); } }


        public ObservableCollection<SchoolModel> AllSchools { get { return SchoolModel.GetAll(); } }

    }
}
