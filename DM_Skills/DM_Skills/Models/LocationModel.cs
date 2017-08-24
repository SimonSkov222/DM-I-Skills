using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Models
{
    [Serializable]
    public class LocationModel : ModelSettings
    {
        const int ERRNO_NAME_NULL = 1;
        private SettingsModel Settings = null;

        public const string ERROR_NAME_NULL = "";

        public int ID { get; set; }
        public string Name { get; set; }

        public LocationModel()
        {
            if (Settings == null)
            {
                Settings = Application.Current.FindResource("Settings") as SettingsModel;
            }
        }

        public override bool CanUpload
        {
            get
            {
                if (Name == null || Name == "")
                {
                    ErrNo = ERRNO_NAME_NULL;
                    Error = ERROR_NAME_NULL;
                    return false;
                }

                return true;
            }
        }

        protected override bool OnUpload()
        {
            var myDB = Scripts.Database.GetDB();

            if (Settings.AllLocations.Count(m=>m.Name.ToLower() == Name.ToLower()) > 0)
            {
                var result = Settings.AllLocations.First(m => m.Name.ToLower() == Name.ToLower());
                ID = result.ID;
                Name = result.Name;
            }
            else
            {
                ID = Convert.ToInt32(myDB.Insert("Locations", "Name", Name, true));
            }
            
            myDB.Disconnect();

            return true;
        }


        public static ObservableCollection<LocationModel> GetAll()
        {
            var result = new ObservableCollection<LocationModel>();
            var db = Scripts.Database.GetDB();
            db.UseDistinct = true;

            var data = db.GetRows("Locations", new string[] { "ID", "Name" }, "ORDER BY `Name`");
            if (data != null)
            {
                foreach (var item in data)
                {
                    result.Add(new LocationModel()
                    {
                        ID = Convert.ToInt32(item[0]),
                        Name = Convert.ToString(item[1])
                    });
                }
            }
            db.Disconnect();

            return result;
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
