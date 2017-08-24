using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using DM_Skills.Scripts;
using System.Windows;

namespace DM_Skills.Models
{
    [Serializable]
    public class SchoolModel : ModelSettings
    {
        static SettingsModel Settings = null;
        public bool SendBroadcast = true;
        const int ERRNO_NAME_NULL = 1;

        public const string ERROR_NAME_NULL = "";


        private string _Name;

        public int ID { get; set; }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged("CanUpload");
                NotifyPropertyOnAll?.Invoke();
            }
        }

        public SchoolModel() {
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
            if (Settings.AllSchools.Count(m=>m.Name.ToLower() == Name.ToLower()) > 0)
            {
                var result = Settings.AllSchools.First(m => m.Name.ToLower() == Name.ToLower());
                ID = result.ID;
                Name = result.Name;
            }
            else
            {
                ID = Convert.ToInt32(myDB.Insert("Schools", "Name", Name, true));
            }

            myDB.Disconnect();
            
            return true;
        }


        public override string ToString()
        {
            return Name;
        }
        //public bool CanUpload() { return false; }
        //public bool Upload() { return false; }
        //public static void GetRow(int id) { }
        //public static void GetResults(int limit, int offset) { }


        public static ObservableCollection<SchoolModel> GetAll()
        {
            var result = new ObservableCollection<SchoolModel>();
            var db = Scripts.Database.GetDB();
            db.UseDistinct = true;

            var data = db.GetRows("Schools", new string[] { "ID", "Name" }, "ORDER BY `Name`");
            if (data != null)
            {
                foreach (var item in data)
                {
                    result.Add(new SchoolModel()
                    {
                        ID = Convert.ToInt32(item[0]),
                        Name = Convert.ToString(item[1])
                    });
                }
            }
            db.Disconnect();

            return result;
        }

        public static void RemoveUnused()
        {
            var myDB = Scripts.Database.GetDB();
            myDB.UseDistinct = true;

            var rows = myDB.GetRows("Teams", "SchoolID");


            var ids = new List<int>();
            if (rows != null)
            {
                foreach (var i in rows)
                {
                    ids.Add(Convert.ToInt32(i[0]));
                }
            }
            myDB.Delete("Schools", "WHERE `ID` NOT IN ({0})", string.Join(", ", ids));
        }

    }
}
