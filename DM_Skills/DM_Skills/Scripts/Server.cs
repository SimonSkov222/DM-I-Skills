﻿//using GodSharp;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Scripts
{
    class Server
    {
        //SocketServer server;
        SimpleTcpServer Host;
        List<System.Net.Sockets.Socket> Clients;

        public Server()
        {
            
        }

        public void Start(int port = 7788)
        {
            Host = new SimpleTcpServer();            
            Host.DataReceived += Host_DataReceived;
            Host.ClientConnected += (o, e) => {
                Console.WriteLine("Client Connected");

                Application.Current.Dispatcher.Invoke(new Action(() => {
                    Models.SettingsModel.lab.Content += "Client Connected\n";
                }));
            };
            Host.Start(port);
            Models.SettingsModel.lab.Content += "Server Started\n";
        }

        private void Host_DataReceived(object sender, Message e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                Models.SettingsModel.lab.Content += "Data Received\n";
            }));
            var packet = Helper.ByteArrayToObject(e.Data) as Packet;
            var reply = new Packet() { ID = packet.ID, Type = packet.Type };

            switch (packet.Type)
            {
                case PacketType.Disconnect:
                    break;
                case PacketType.GetSchools:
                    reply.Data = Models.SchoolModel.GetAll();
                    break;
                case PacketType.GetLocations:
                    break;
                case PacketType.UploadTables:
                    break;
                case PacketType.Search:
                    break;
                case PacketType.QuerySQL:
                    var myDB = Database.GetDB();

                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        Models.SettingsModel.lab.Content += $"Client Query:\n {packet.Data.ToString()}\n";
                    }));

                    var dt = myDB.ExecuteQuery(packet.Data as string);
                    reply.Data = dt;


                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        Models.SettingsModel.lab.Content += $"Rows: \n {dt.Count}\n";
                    }));
                    myDB.Disconnect();
                    break;
                default:
                    break;
            }

            e.Reply(Helper.ObjectToByteArray(reply));
        }

        public void Disconnect()
        {
           // server.Close();
        }

        private void OnReceiveData(System.Net.Sockets.Socket socket, byte[] data)
        {
            var packet = Helper.ByteArrayToObject(data) as Packet;
            var reply = new Packet() { ID = packet.ID, Type = packet.Type };

            switch (packet.Type)
            {
                case PacketType.Disconnect:
                    break;
                case PacketType.GetSchools:
                    reply.Data = Helper.ObjectToByteArray(new System.Collections.ObjectModel.ObservableCollection<object>() { "Hej",42,"asd" });
                    socket.Send(Helper.ObjectToByteArray(reply));
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
        }

        private void OnClientConnect(System.Net.Sockets.Socket socket)
        {
            Clients.Add(socket);
        }

    }
}