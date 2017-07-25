using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    class SchoolModel : ModelSettings
    {
        public int ID { get; set; }
        public string Name { get; set; }


        protected override bool OnUpload()
        {
            Console.WriteLine("Upload School");
            return true;
        }

        //public bool CanUpload() { return false; }
        //public bool Upload() { return false; }
        //public static void GetRow(int id) { }
        //public static void GetResults(int limit, int offset) { }
    }
}
