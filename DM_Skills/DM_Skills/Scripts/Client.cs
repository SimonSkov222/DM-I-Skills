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
            Application.Current.Exit += Current_Exit;
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
                        if (Settings.IsClient)
                        {
                            var reply = client.WriteLineAndGetReply("875120", new TimeSpan(0, 0, 0, 0, 300));
                            if (reply == null)
                            {
                                Console.WriteLine("Is Disconnected");
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
                Console.WriteLine(e.Message);
                Settings.IsClient = false;
            }


            return Settings.IsClient;
        }


       

        public void Disconnect() {
            pingServer.Abort();
            CloseConnection();
            Settings.InvokeDisconnection(true);
        }

        public void Send(PacketType type, Action<object> cb = null, object data = null)
        {
            Console.WriteLine("Start sending...");
            waitHandel.Reset();
            var packet = new Packet()
            {
                Type = type,
                Data = data
            };
            do
            {
                packet.ID = r.Next(1000, 100000);
            } while (Callbacks.ContainsKey(packet.ID));
            
            Callbacks.Add(packet.ID, cb);       
            
            client.Write(Helper.ObjectToByteArray(packet));

            if(packet.Type == PacketType.Read)
            {
                Console.WriteLine("Sending Wait");
                waitHandel.WaitOne();
            }
            Console.WriteLine("Sending Done");
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


            
            Console.WriteLine("ReplayDone");

        }

        private void Current_Exit(object sender, ExitEventArgs e)
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
