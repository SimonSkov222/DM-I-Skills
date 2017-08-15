//using GodSharp;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class Client
    {
        //SocketClient client;
        SimpleTcpClient client;
        Random r = new Random(157);
        Dictionary<int, Action<object>> Callbacks = new Dictionary<int, Action<object>>();

        public void Connect(string ipAddress = "127.0.0.1", int port = 7788) {
            client = new SimpleTcpClient();
            client.DataReceived += Client_DataReceived;
            client.Connect(ipAddress, port);
        }

        private void Client_DataReceived(object sender, Message e)
        {

            Console.WriteLine("Got Reply!!");

            var packet = Helper.ByteArrayToObject(e.Data) as Packet;

            switch (packet.Type)
            {
                case PacketType.Disconnect:
                    break;
                case PacketType.GetSchools:
                    break;
                case PacketType.GetLocations:
                    break;
                case PacketType.UploadTables:
                    break;
                case PacketType.Search:
                    break;
                default:
                    break;
            }



            if (Callbacks.ContainsKey(packet.ID))
            {
                if (Callbacks[packet.ID] != null)
                {
                    Callbacks[packet.ID](packet.Data);
                }
            }
        }

        public void Disconnect() {
            //client.Close();
        }

        public void Send(PacketType type, Action<object> cb = null, object data = null)
        {
            var packet = new Packet()
            {
                ID = r.Next(1000, 100000),
                Type = type,
                Data = data
            };
            
            Callbacks.Add(packet.ID, cb);


            var packetAsBytes = Helper.ObjectToByteArray(packet);
            client.Write(packetAsBytes);
            //client.Send(packetAsBytes);
        }
        

    }
}
