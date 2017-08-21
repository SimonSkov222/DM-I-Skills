//using GodSharp;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Scripts
{
    class Client
    {
        private Models.SettingsModel Settings;
        //SocketClient client;
        SimpleTcpClient client;
        Random r = new Random(157);
        Dictionary<int, Action<object>> Callbacks = new Dictionary<int, Action<object>>();
        private ManualResetEvent waitHandel = new ManualResetEvent(false);
        Thread pingServer;

        public Client()
        {
            Settings = (Models.SettingsModel)Application.Current.FindResource("Settings");
            Application.Current.MainWindow.Closed += MainWindow_Closed;
        }
        


        public bool Connect(string ipAddress = "127.0.0.1", int port = 7788) {
            try
            {
                client = new SimpleTcpClient();
                client.DataReceived += Client_DataReceived;
                client.Connect(ipAddress, port);

                pingServer = new Thread(new ThreadStart(delegate () {
                    while (true)
                    {
                        Thread.Sleep(5000);
                        if (Settings.IsClient && client.TcpClient.Connected)
                        {
                            Message reply;
                            try
                            {
                                reply = client.WriteLineAndGetReply("#875120", new TimeSpan(0, 0, 0, 0, 300));
                            }
                            catch (Exception) { reply = null; }

                            if (reply == null)
                            {
                                Application.Current.Dispatcher.Invoke(delegate ()
                                {

                                    CloseConnection();
                                    Settings.InvokeDisconnection(false);
                                });
                                break;
                            }
                        }
                    }
                }));
                pingServer.Start();
                Settings.IsClient = true;
                Settings.InvokeConnection();
            }
            catch (Exception e)
            {
                Settings.IsClient = false;
            }


            return Settings.IsClient;
        }


       

        public void Disconnect() {
            pingServer.Abort();
            CloseConnection();
            Settings.InvokeDisconnection(true);
        }

        public void Send(PacketType type, Action<object> cb = null, object data = null, PacketType? broadcast = null)
        {
            waitHandel.Reset();
            var packet = new Packet()
            {
                Type = type,
                Data = data,
                BroadcastType = broadcast
            };
            do
            {
                packet.ID = r.Next(1000, 100000);
            } while (Callbacks.ContainsKey(packet.ID));
            
            Callbacks.Add(packet.ID, cb);       
            
            client.Write(Helper.ObjectToByteArray(packet));

            if(packet.Type == PacketType.Read)
            {
                waitHandel.WaitOne();
            }
        }
        
        private void Client_DataReceived(object sender, Message e)
        {
            Console.Write("Got Reply..");
            var packet = Helper.ByteArrayToObject(e.Data) as Packet;

            switch (packet.Type)
            {
                case PacketType.Read:
                    if (Callbacks.ContainsKey(packet.ID))
                    {
                        Callbacks[packet.ID]?.Invoke(packet.Data);
                    }
                    waitHandel.Set();
                    break;
                case PacketType.Broadcast_UploadTables:
                    Application.Current.Dispatcher.Invoke(delegate() 
                    {
                        Settings.NotifyPropertyChanged(nameof(Settings.AllSchools));
                        Settings.InvokeUpload();
                    });
                    break;
                case PacketType.Boardcast_UploadSchools:
                    Application.Current.Dispatcher.Invoke(delegate ()
                    {
                        Settings.NotifyPropertyChanged(nameof(Settings.AllSchools));
                    });
                    break;
            }


            

        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (pingServer != null) {
                pingServer.Abort();
            }
            CloseConnection();
        }
        private void CloseConnection() {
            client.Dispose();
            client.Disconnect();
            Settings.IsClient = false;
        }
    }
}
