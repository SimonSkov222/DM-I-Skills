using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    public class SchoolModel : ModelSettings
    {
        const int ERRNO_NAME_NULL = 1;

        public const string ERROR_NAME_NULL = "";

        public int ID { get; protected set; }
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
    }
}
