using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    interface IDataReceive
    {

        bool OnData(object sender, int command, object data, out object reply);
    }
}
