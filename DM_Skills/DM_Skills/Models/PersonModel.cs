using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    class PersonModel : ModelSettings
    {

        const int ERRNO_NAME_NULL = 1;

        public const string ERROR_NAME_NULL = "";



        public int ID { get; protected set; }
        public string Name { get; set; }
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
            throw new NotImplementedException();
        }
    }
}
