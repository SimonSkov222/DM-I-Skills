using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DM_Skills.Scripts
{
    class JsonClient : JsonTCP
    {
        public event Action<string> Debug_Output;
        
        public event Action OnConnected;
        public event Action<bool> OnDisconnected;

        //SocketClient client;
        private SimpleTcpClient client;
        private Dictionary<int, Action<object>> Callbacks = new Dictionary<int, Action<object>>();
        //private Thread pingServer;

        private DispatcherTimer disconnectTimer = new DispatcherTimer();

        private TimeSpan timeout = new TimeSpan(0,0,35);

        public JsonClient(IConverterJsonTCP converterJson, IDataReceive dataController)
        {
            JsonConverter = converterJson;
            DataController = dataController;
            //Application.Current.LoadCompleted += (o, e) => Application.Current.MainWindow.Closed += Program_Exit;
            //Application.Current.Exit += Program_Exit;
            //pingServer = new Thread(new ThreadStart(Ping_Method));

            disconnectTimer.Tick += DisconnectTimer_Tick;
            disconnectTimer.Interval = timeout;
        }
        



        /////////////////////////////////////
        //          Public
        /////////////////////////////////////

        public bool Connect(string ipAddress = "127.0.0.1", int port = 7788)
        {
            try
            {
                client = new SimpleTcpClient();
                client.Delimiter = 10;
                //client.DelimiterDataReceived += (o, e) => { Console.WriteLine("Delimiter data received"); };
                client.DelimiterDataReceived += DataReceived;
                //client.DataReceived += DataReceived;

                client.Connect(ipAddress, port);

                disconnectTimer.Start();
                //pingServer.Start();

                InvokeOutput("Client Connected.");
                Application.Current.MainWindow.Closed += MainWindow_Closed;


                OnConnected?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning#001: " + ex.Message);
                InvokeOutput("Client Not Connected.");
                return false;
            }
            
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Close connection");
            CloseConnection(false);
        }

        public void Disconnect()
        {
            Application.Current.MainWindow.Closed -= MainWindow_Closed;
            //pingServer.Abort();
            Send(COMMAND_DISCONNECT);
            CloseConnection(true);
        }

        public void Send(int command, object data = null, Action<object> cb = null)
        {
            int packetID = SaveCallback(cb);
            var packet = PackJson(command, packetID, data);
            try
            {
                client.Write(packet + client.StringEncoder.GetString(new byte[] { client.Delimiter }));

                InvokeOutput($"Packet Send: {packet}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning#002: " + ex.Message);
                InvokeOutput($"Packet Not Send: {packet}");
            }
        }

        public void Broadcast(int command)
        {
            Send(command);
        }

        /////////////////////////////////////
        //          Private
        /////////////////////////////////////

        private int SaveCallback(Action<object> callback)
        {
            int id = -1;
            if (callback != null)
            {
                lock (Callbacks)
                {
                    id = Callbacks.Count;
                    while (Callbacks.ContainsKey(id))
                    {
                        id++;
                    }
                    Callbacks.Add(id, callback);
                }
            }

            return id;
        }

        private void InvokeCallback(int id, object data)
        {
            lock (Callbacks)
            {
                if (Callbacks.ContainsKey(id))
                {
                    Callbacks[id]?.Invoke(data);
                    Callbacks.Remove(id);
                }
            }
        }

        private void CloseConnection(bool byUser)
        {
            disconnectTimer.Stop();
            if (client != null)
            {
                client.Disconnect();
            }
            Callbacks.Clear();
            InvokeOutput("Client Disconnected.");
            OnDisconnected?.Invoke(byUser);
        }
        
        private void InvokeOutput(string text) {
            Console.WriteLine("Client: "+text);
            //Debug_Output?.Invoke(text);
            //Helper.ProcessUITasks();
        }

        /////////////////////////////////////
        //          Events
        /////////////////////////////////////

        private void DataReceived(object sender, Message e)
        {
            Console.WriteLine("Got packet");
            var packet = UnpackJson(e.MessageString);

            switch ((int)packet[0])
            {
                case COMMAND_PING: disconnectTimer.Stop(); disconnectTimer.Start(); Console.WriteLine("Ping");       return;
                case COMMAND_DISCONNECT:    CloseConnection(false);                                 return;
            }
            
            
            InvokeOutput($"Packet Resived: {e.MessageString}");
            //Console.WriteLine($"Need Reply {DataController.OnData((int)packet[0], packet[3], out object reply)}");
            if (DataController.OnData(this, (int)packet[0], packet[3], out object reply))
            {
                var replyPacket = PackJson((int)packet[0], (int)packet[1], reply);
                try
                {
                    e.Reply(replyPacket + client.StringEncoder.GetString(new byte[] { client.Delimiter }));
                    InvokeOutput($"Reply Send: {replyPacket}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning#003: " + ex.Message);
                    InvokeOutput($"Reply Not Send: {replyPacket}");
                }
            }

            InvokeOutput($"Call Callback");
            InvokeCallback((int)packet[1], packet[3]);
        }
        
        private void Program_Exit(object sender, EventArgs e)
        {
            //if (pingServer != null)
            //{
            //    pingServer.Abort();
            //}
            CloseConnection(true);
        }


        private void DisconnectTimer_Tick(object sender, EventArgs e)
        {
            disconnectTimer.Stop();
            InvokeOutput($"No Ping From Server");
            var answer = MessageBox.Show("Kunne ikke få svar fra serveren.\nVil du vente på et svar?", "Timeout", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer == MessageBoxResult.Yes)
            {
                disconnectTimer.Start();
            }
            else
            {
                CloseConnection(false);
            }


            //while (client.TcpClient.Connected)
            //{
            //    Thread.Sleep(3000);
            //    try
            //    {
            //        var packet = PackJson(COMMAND_PING);
            //        Message reply = client.WriteLineAndGetReply(packet, new TimeSpan(0, 0, 0, 4000, 0));

            //        if (reply == null)
            //        {
            //            InvokeOutput($"Ping Send And Got No Reply: {packet}");
            //            var answer = MessageBox.Show("Kunne ikke få svar fra serveren.\nVil du vente på et svar?", "Timeout", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            //            if (answer == MessageBoxResult.No)
            //            {
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            InvokeOutput($"Ping Send And Got Reply: {packet}");
            //        }

            //    }
            //    catch (Exception)
            //    {

            //        InvokeOutput($"Ping Not Send");
            //        break;

            //    }
            //}

            //InvokeOutput($"Ping Stopped");
            //CloseConnection(false);
        }


    }
}
