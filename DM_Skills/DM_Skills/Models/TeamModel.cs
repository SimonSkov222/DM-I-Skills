using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    public class TeamModel : ModelSettings
    {
        const int ERRNO_TIME_NULL = 1;

        public const string ERROR_TIME_NULL = "";
        
        public int? ID { get; set; }
        public int SchoolID { get; set; }
        public int LocationID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Class { get; set; }



        public override bool CanUpload
        {
            get
            {
                if (Time == null || Time == "")
                {
                    ErrNo = ERRNO_TIME_NULL;
                    Error = ERROR_TIME_NULL;
                    return false;
                }

                return true;
            }
        }


        protected override bool OnUpload()
        {
            Console.WriteLine("1");

            if (Date == null || Date == "")
            {
                Date = DateTime.Now.ToShortDateString();
                Console.WriteLine("2");
            }

            Console.WriteLine("3");
            var myDB = Scripts.Database.GetDB();
            Console.WriteLine("4");
            if (ID != null)
            {
                return false;
            }
            else
            {
                Console.WriteLine("6");
                ID = (int)myDB.Insert("Teams", new string[] { "Class", "SchoolID", "LocationID", "Date", "Time"}, new string[] { Class });

            }
            myDB.Disconnect();
            Console.WriteLine("7");
            return true;


            //return true;


        }
    }
}
