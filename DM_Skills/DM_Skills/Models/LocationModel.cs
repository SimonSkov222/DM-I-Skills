using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    [Serializable]
    public class LocationModel : ModelSettings
    {
        const int ERRNO_NAME_NULL = 1;

        public const string ERROR_NAME_NULL = "";

        public int ID { get; set; }
        public string Name { get; set; }

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
            Console.WriteLine("Upload Location");
            var myDB = Scripts.Database.GetDB();

            if (myDB.Exist("Locations", "Name", Name))
            {
                var result = myDB.GetRow("Locations", "ID", "WHERE `Name` = '{0}'", Name);
                ID = Convert.ToInt32(result[0]);
            }
            else
            {
                ID = Convert.ToInt32(myDB.Insert("Locations", "Name", Name, true));
            }


            Console.WriteLine("Upload Location->" + ID);
            myDB.Disconnect();

            return true;
        }


        public static ObservableCollection<LocationModel> GetAll()
        {
            var result = new ObservableCollection<LocationModel>();
            var db = Scripts.Database.GetDB();
            db.UseDistinct = true;

            var data = db.GetRows("Locations", new string[] { "ID", "Name" });
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
