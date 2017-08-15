using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    public class PersonModel : ModelSettings
    {

        const int ERRNO_NAME_NULL = 1;

        public const string ERROR_NAME_NULL = "";



        public int? ID { get; set; }

        private string _Name;
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
        public int TeamID { get; set; }

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

            if (ID != null)
            {
                return false;
            }
            else
            {
                ID = Convert.ToInt32(myDB.Insert("Persons", new string[] { "Name", "Teamid" }, new object[] { Name, TeamID }));
            }
            myDB.Disconnect();

            return true;
        }

        public static ObservableCollection<PersonModel> GetAll()
        {
            var result = new ObservableCollection<PersonModel>();
            var db = Scripts.Database.GetDB();
            db.UseDistinct = true;

            var data = db.GetRows("Person", new string[] { "ID", "Name" });
            foreach (var item in data)
            {
                result.Add(new PersonModel()
                {
                    ID = (int)item[0],
                    TeamID = (int)item[1],
                    Name = (string)item[2]
                });
            }
            db.Disconnect();

            return result;
        }
    }
}
