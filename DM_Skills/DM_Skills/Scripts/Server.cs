//using GodSharp;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Scripts
{
    class Server
    {
        //SocketServer server;
        private Models.SettingsModel Settings;
        private List<TcpClient> Clients = new List<TcpClient>();
        SimpleTcpServer Host;

        public Server()
        {
            Settings = (Models.SettingsModel)Application.Current.FindResource("Settings");
            Application.Current.MainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        public void Start(int port = 7788)
        {
            try
            {
                Host = new SimpleTcpServer();
                Host.DataReceived += Host_DataReceived;
                Host.ClientConnected += (o, e) => 
                {

                    for (int i = Clients.Count-1; i >= 0; i--)
                    {
                        if (!Clients[i].Connected)
                        {
                            Clients.RemoveAt(i);
                        }
                    }

                    Clients.Add(e);
                    Console.WriteLine("Client Connected");
                };

            
            
                Host.Start(port);
                Settings.IsServer = true;
                Settings.InvokeConnection();
            }
            catch (Exception)
            {
                Settings.IsServer = false;
                MessageBox.Show("Kunne ikke starte serveren.\nTjek at porten ikke bliver brugt.", "Server ikke startet", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }

        public void Stop()
        {
            Host.Stop();
            Host = null;
            Settings.IsServer = false;
            Settings.InvokeDisconnection(true);
        }


        public void Broadcast(PacketType type, object data = null)
        {
            var packet = new Packet()
            {
                Type = type,
                ID = -100,
                Data = data
            };
            try
            {
                MyBroadcast(Helper.ObjectToByteArray(packet));
            }
            catch (Exception)
            {
                Broadcast(type, data);
            }
            

            new Thread(new ThreadStart(delegate ()
            {
                Application.Current.Dispatcher.Invoke(delegate ()
                {
                    switch (type)
                    {
                        case PacketType.Broadcast_UploadTables:
                            Settings.InvokeSchoolsChanged();
                            Settings.InvokeUpload();
                            break;
                        case PacketType.Broadcast_UploadSchools:
                            Settings.InvokeSchoolsChanged();
                            break;
                    }
                });
            })).Start();

        }

        private void Host_DataReceived(object sender, Message e)
        {
            Packet packet;

            try
            {
                packet = Helper.ByteArrayToObject(e.Data) as Packet;
            }
            catch (Exception)
            {
                //Code fundet i Client.cs
                var code = System.Text.RegularExpressions.Regex.Match(e.MessageString, "#\\d+").Value;
                if (code == "#875120")
                {
                    e.Reply(Helper.ObjectToByteArray(new Packet() { Type = PacketType.Ping }));
                }
                return;
            }
            var reply = new Packet() { ID = packet.ID, Type = packet.Type };

            var myDB = Database.GetDB();
            switch (packet.Type)
            {
                case PacketType.GetLocation:
                    reply.Type = PacketType.Broadcast_LocationChanged;
                    reply.Data = Settings.Location;
                    e.Reply(Helper.ObjectToByteArray(reply));
                    break;
                case PacketType.MultipleQuery:
                    myDB.MultipleQuery = true;
                    myDB.Querys = packet.Data as List<string>;
                    myDB.ExecuteALL();
                    e.Reply(Helper.ObjectToByteArray(reply));
                    break;
                case PacketType.Read:

                    var dt = myDB.ExecuteQuery(packet.Data as string);
                    reply.Data = dt;
                    e.Reply(Helper.ObjectToByteArray(reply));
                    break;
                case PacketType.Write:
                    myDB.ExecuteQuery(packet.Data as string);
                    e.Reply(Helper.ObjectToByteArray(reply));
                    break;
                case PacketType.Broadcast_UploadTables:
                    Broadcast(packet.Type);
                    break;
                case PacketType.Broadcast_UploadSchools:
                    Broadcast(packet.Type);
                    break;
            }
            myDB.Disconnect();

        }



        private void MyBroadcast(byte[] data)
        {
            foreach (var client in Clients.Where(x => x.Connected))
            {
                client.GetStream().Write(data, 0, data.Length);
            }
            
        }


    }
}
