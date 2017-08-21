using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    public enum PacketType {
        Broadcast_UploadTables,
        Boardcast_UploadSchools,
        Write,
        Read,
        Ping
    }

    [Serializable]
    public class Packet
    {
        public int ID { get; set; }
        public PacketType Type { get; set; }
        public PacketType? BroadcastType { get; set; }
        public object Data { get; set; }

    }
}
