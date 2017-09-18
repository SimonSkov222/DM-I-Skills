using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    interface IConverterJsonTCP
    {
        object From(string key, Dictionary<string, object> data);
        string To(out string key, object data);

    }
}
