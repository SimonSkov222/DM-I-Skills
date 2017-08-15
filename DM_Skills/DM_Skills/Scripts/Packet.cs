using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    public enum PacketType {
        Disconnect,
        GetSchools,
        GetLocations,
        UploadTables,
        Search
    }

    [Serializable]
    public class Packet
    {
        public int ID { get; set; }
        public PacketType Type { get; set; }
        public object Data { get; set; }

    }
}
