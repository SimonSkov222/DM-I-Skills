using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    public class SchoolModel : ModelSettings
    {


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

            if (myDB.Exist("Schools", "Name", Name))
            {
                var result = myDB.GetRow("Schools", "ID", "WHERE [Name] = '{0}'", Name);
                ID = (int)result[0];
            }
            else
            {
                ID = (int)myDB.Insert("Schools", "Name", Name);
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

            var data = db.GetRows("Schools", new string[] { "ID", "Name" });
            foreach (var item in data)
            {
                result.Add(new SchoolModel()
                {
                    ID = (int)item[0],
                    Name = (string)item[1]
                });
            }
            db.Disconnect();

            return result;
        }
    }
}
