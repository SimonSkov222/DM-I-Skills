using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    public enum PacketType {
        Broadcast_UploadTables,
        Broadcast_UploadSchools,
        MultipleQuery,
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
