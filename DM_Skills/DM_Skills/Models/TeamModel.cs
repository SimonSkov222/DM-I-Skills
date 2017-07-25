using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    class TeamModel : ModelSettings
    {

        public int ID { get; set; }
        public int SchoolID { get; set; }
        public int LocationID { get; set; }
        public int TableID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Class { get; set; }



        protected override bool OnUpload()
        {
            Console.WriteLine("Upload TeamModel");
            return true;
        }
    }
}
