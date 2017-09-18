using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class DataReceivedServer : IDataReceive
    {
        public bool OnData(object sender, int command, object data, out object reply)
        {
            Console.WriteLine("Got Data");
            reply = null;

            switch (command)
            {
                case (int)JsonCommandIDs.Read:
                    Console.WriteLine("reply on read");
                    reply = Database.GetDB().ExecuteQuery((string)data);
                    return true;
                case (int)JsonCommandIDs.Write:
                    Console.WriteLine("reply on write");
                    Database.GetDB().ExecuteQuery((string)data);
                    return true;


                case (int)JsonCommandIDs.Broadcast_LocationChanged:     ((JsonServer)sender).Broadcast((int)JsonCommandIDs.Broadcast_LocationChanged, data); break;
                case (int)JsonCommandIDs.Broadcast_TimerStarted:        ((JsonServer)sender).Broadcast((int)JsonCommandIDs.Broadcast_TimerStarted); break;

                case (int)JsonCommandIDs.Broadcast_UploadSchools:
                    ((JsonServer)sender).Broadcast((int)JsonCommandIDs.Broadcast_UploadSchools);
                    Models.SettingsModel.Singleton.InvokeSchoolsChanged();

                    break;

                case (int)JsonCommandIDs.Broadcast_UploadTables:
                    ((JsonServer)sender).Broadcast((int)JsonCommandIDs.Broadcast_UploadTables);
                    Models.SettingsModel.Singleton.InvokeUpload();
                    break;
            }


            return false;
        }
    }
}
