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
        private SimpleTcpClient client;
        private Random r = new Random(157);
        private Dictionary<int, Action<object>> Callbacks = new Dictionary<int, Action<object>>();
        private ManualResetEvent waitHandel = new ManualResetEvent(false);
        private Thread pingServer;

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
               // client.TcpClient.Client..SetSocketOption(System.Net.Sockets.SocketOptionLevel.Tcp, System.Net.Sockets.SocketOptionName.KeepAlive, true);

                client.Connect(ipAddress, port);

                pingServer = new Thread(new ThreadStart(delegate () {
                    while (true)
                    {
                        Thread.Sleep(25000);
                        if (Settings.IsClient && client.TcpClient.Connected)
                        {
                            Message reply;
                            try
                            {
                                reply = client.WriteLineAndGetReply("#875120", new TimeSpan(0, 0, 0, 15, 0));
                            }
                            catch (Exception)
                            { reply = null; }

                            if (reply == null)
                            {
                                var answer = MessageBox.Show("Kunne ikke få svar fra serveren.\nVil du vente på et svar?", "Timeout", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (answer == MessageBoxResult.No)
                                {
                                    Application.Current.Dispatcher.Invoke(delegate ()
                                    {
                                        Console.WriteLine("Disconnect");
                                        CloseConnection();
                                        Settings.InvokeDisconnection(false);
                                    });
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(delegate ()
                            {
                                Console.WriteLine("Disconnect");
                                CloseConnection();
                                Settings.InvokeDisconnection(false);
                            });
                            break;

                        }
                    }
                    
                }));
                pingServer.Start();

                Settings.IsClient = true;
                Settings.InvokeConnection();
            }
            catch (Exception)
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

        public void Send(PacketType type, Action<object> cb = null, object data = null)
        {

            var packet = new Packet()
            {
                Type = type,
                Data = data
            };
            lock (Callbacks)
            {
                packet.ID = Callbacks.Count;
                while (Callbacks.ContainsKey(packet.ID))
                {
                    packet.ID++;
                }
                Callbacks.Add(packet.ID, cb);   
            }

            try
            {
                client.Write(Helper.ObjectToByteArray(packet));
            }
            catch (Exception) { MessageBox.Show("Kan ikke sende til serveren"); }
        }
        
        private void Client_DataReceived(object sender, Message e)
        {
            
            var packet = Helper.ByteArrayToObject(e.Data) as Packet;
            
            lock (Callbacks)
            {
                if (Callbacks.ContainsKey(packet.ID))
                {
                    
                    Callbacks[packet.ID]?.Invoke(packet.Data);
                    Callbacks.Remove(packet.ID);
                }
            }
            (new Thread(new ThreadStart(delegate () 
            { 
                switch (packet.Type)
                {
                    case PacketType.Broadcast_UploadTables:
                        Application.Current.Dispatcher.Invoke(delegate() 
                        {
                            Settings.InvokeSchoolsChanged();
                            Settings.InvokeUpload();
                        });
                        break;
                    case PacketType.Broadcast_UploadSchools:
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            Settings.InvokeSchoolsChanged();
                        });
                        break;
                    case PacketType.Broadcast_LocationChanged:
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            Settings.Location = packet.Data as Models.LocationModel;
                            Settings.InvokeLocationChanged(null);
                        });
                        break;
                    case PacketType.Broadcast_TimerStarted:
                        Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            Settings.InvokeTimerStarted();
                        });
                        break;
                }
            }))).Start();

            
        }

        public void Broadcast(PacketType type)
        {
            var packet = new Packet()
            {
                ID = -1,
                Type = type,
                Data = null
            };

            

            client.Write(Helper.ObjectToByteArray(packet));
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
