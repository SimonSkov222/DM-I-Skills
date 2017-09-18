using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DM_Skills.Scripts
{

    class JsonServer : JsonTCP
    {
        public event Action<string> Debug_Output;

        public event Action OnStarted;
        public event Action OnStopped;


        private List<TcpClient> Clients = new List<TcpClient>();
        private SimpleTcpServer Host;

        private DispatcherTimer disconnectPing = new DispatcherTimer();

        private TimeSpan timeoutPing = new TimeSpan(0,0,30);

        public JsonServer(IConverterJsonTCP converterJson, IDataReceive dataController)
        {
            JsonConverter = converterJson;
            DataController = dataController;

            disconnectPing.Tick += (o, e) => { Broadcast(COMMAND_PING); InvokeOutput("Broadcast Ping"); };
            disconnectPing.Interval = timeoutPing;


            //Application.Current.LoadCompleted += (o, e) => Application.Current.MainWindow.Closed += Program_Exit;
            //Application.Current.Exit += Program_Exit;
        }
        

        /////////////////////////////////////
        //          Public
        /////////////////////////////////////

        public bool Start(int port = 7788)
        {
            try
            {
                Host = new SimpleTcpServer();
                Host.Delimiter = 0x10;
                Host.DelimiterDataReceived += (o, e) => { Console.WriteLine("Delimter data received"); };
                Host.DataReceived += DataReceived;
                Host.ClientConnected += ClientConnected;

                Host.Start(port);

                disconnectPing.Start();
                InvokeOutput("Server Started.");
                OnStarted?.Invoke();


                return true;
            }
            catch (Exception)
            {
                InvokeOutput("Server Not Started.");
                return false;
            }
        }
        
        public void Stop()
        {
            disconnectPing.Stop();
            Broadcast(COMMAND_DISCONNECT);
            Host.Stop();
            Host = null;

            foreach (var client in Clients)
            {
                DisconnectClient(client);
            }

            Clients.Clear();

            InvokeOutput("Server Stopped.");
            OnStopped?.Invoke();
        }
        
        public void Broadcast(int command, object data = null)
        {
            var packet = PackJson(command, null, data);            

            var dataInBytes = Host.StringEncoder.GetBytes(packet);
            foreach (var client in Clients.Where(x => x.Connected))
            {
                try { client.GetStream().Write(dataInBytes, 0, dataInBytes.Length); }
                catch (Exception) { client.Close(); client.Dispose(); }
            }
            InvokeOutput($"Broadcast Send: {packet}");
        }

        /////////////////////////////////////
        //          Private
        /////////////////////////////////////

        private void MyBroadcast(byte[] data)
        {
            foreach (var client in Clients.Where(x => x.Connected))
            {
                client.GetStream().Write(data, 0, data.Length);
            }

        }

        private void InvokeOutput(string text)
        {
            Console.WriteLine("Server: " +text);
        }

        private void DisconnectClient(TcpClient client) {
            client.Close();
            client.Dispose();
        }

        /////////////////////////////////////
        //          Events
        /////////////////////////////////////

        private void DataReceived(object sender, Message e)
        {
            InvokeOutput($"Packet Resived: {e.MessageString}");
            var packet = UnpackJson(e.MessageString);

            switch ((int)packet[0])
            {
                case COMMAND_PING:          e.Reply(e.MessageString);           return;
                case COMMAND_DISCONNECT:    DisconnectClient(e.TcpClient);      return;
            }

            if (DataController.OnData(this, (int)packet[0], packet[3], out object reply))
            {
                var replyPacket = PackJson((int)packet[0], (int)packet[1], reply);
                try
                {
                    e.Reply(replyPacket);
                    InvokeOutput($"Reply Send: {replyPacket}");
                }
                catch (Exception)
                {
                    InvokeOutput($"Reply Not Send: {replyPacket}");
                }
            }
            else {

                InvokeOutput($"Reply No Send");
            }
            
        }

        private void ClientConnected(object sender, TcpClient e)
        {
            //Fjern disconnected clients
            for (int i = Clients.Count - 1; i >= 0; i--)
            {
                if (!Clients[i].Connected)
                {
                    //Clients.RemoveAt(i);
                    Console.WriteLine("Remove client");
                }
            }


            Clients.Add(e);
            InvokeOutput("Client Connected.");
        }
        
        private void Program_Exit(object sender, EventArgs e)
        {
            Stop();
        }


    }
}
