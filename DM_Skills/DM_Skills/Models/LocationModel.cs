using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
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
            var myDB = Scripts.Database.GetDB();

            if (myDB.Exist("Locations", "Name", Name))
            {
                var result = myDB.GetRow("Locations", "ID", "WHERE [Name] = '{0}'", Name);
                ID = (int)result[0];
            }
            else
            {
                ID = (int)myDB.Insert("Locations", "Name", Name);
            }
            myDB.Disconnect();

            return true;
        }


        public static ObservableCollection<LocationModel> GetAll()
        {
            var result = new ObservableCollection<LocationModel>();
            var db = Scripts.Database.GetDB();
            db.UseDistinct = true;

            var data = db.GetRows("Locations", new string[] { "ID", "Name" });
            foreach (var item in data)
            {
                result.Add(new LocationModel()
                {
                    ID = (int)item[0],
                    Name = (string)item[1]
                });
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
