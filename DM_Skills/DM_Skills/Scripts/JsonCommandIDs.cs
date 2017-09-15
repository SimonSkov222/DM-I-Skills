using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    public enum JsonCommandIDs
    {
        Broadcast_UploadTables = 2,
        Broadcast_UploadSchools = 3,
        Broadcast_LocationChanged = 4,
        Broadcast_TimerStarted = 5,
        GetLocation = 6,
        MultipleQuery = 7,
        Write = 8,
        Read =9,
        Ping = 10
    }
}
