﻿//using GodSharp;
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
        private bool WaitForReply = false;

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
                                    Console.WriteLine("Disconnect");
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            client.Write(Helper.ObjectToByteArray(packet));

            //if(packet.Type == PacketType.Read || waitForReply)
            //{
            //    WaitForReply = true;
            //    Console.WriteLine("Reset");
            //    waitHandel.Reset();
            //    Console.WriteLine("WaitOne");
            //    waitHandel.WaitOne();
            //}
        }
        
        private void Client_DataReceived(object sender, Message e)
        {
            
            var packet = Helper.ByteArrayToObject(e.Data) as Packet;
            Console.WriteLine("Got reply -" + packet.Type);

            //if (WaitForReply && packet.Type != PacketType.Ping)
            //{
            //    WaitForReply = false;
            //    waitHandel.Set();
            //}
            lock (Callbacks)
            {
                if (Callbacks.ContainsKey(packet.ID))
                {

                    Console.WriteLine($"Reply Callbacks - {packet.Type} ID: {packet.ID}");
                    Callbacks[packet.ID]?.Invoke(packet.Data);
                    Callbacks.Remove(packet.ID);
                }
            }
            (new Thread(new ThreadStart(delegate () { 
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
