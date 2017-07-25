using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    class TableModelN : ModelSettings
    {
        public SchoolModel      School      { get; set; }
        public LocationModel    Location    { get; set; }
        public TeamModel        Team        { get; set; }
        public List<PersonModel> Persons    { get; set; }

        public TableModelN()
        {
            School      = new SchoolModel();
            Location    = new LocationModel();
            Team        = new TeamModel();
            Persons     = new List<PersonModel>();
        }

        public override bool CanUpload {
            get
            {
               // bool canPersonsUpload = Persons.Count((o) => { return o.; }) == 1;

                return true;
            }
        }

        protected override bool OnUpload()
        {
            return false;
        }
    }
}
