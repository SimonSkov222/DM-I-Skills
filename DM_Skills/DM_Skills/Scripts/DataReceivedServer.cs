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
            reply = null;

            switch (command)
            {
                case (int)JsonCommandIDs.GetLocation:   reply = Models.SettingsModel.Singleton.Location;        return true;
                case (int)JsonCommandIDs.Read:          reply = Database.GetDB().ExecuteQuery((string)data);    return true;
                case (int)JsonCommandIDs.Write:         Database.GetDB().ExecuteQuery((string)data);            return true;
                    
                case (int)JsonCommandIDs.Broadcast_LocationChanged:     ((JsonServer)sender).BroadcastLine((int)JsonCommandIDs.Broadcast_LocationChanged, data); break;
                case (int)JsonCommandIDs.Broadcast_TimerStarted:        ((JsonServer)sender).BroadcastLine((int)JsonCommandIDs.Broadcast_TimerStarted); break;


                case (int)JsonCommandIDs.Broadcast_UploadSchools:
                    ((JsonServer)sender).BroadcastLine((int)JsonCommandIDs.Broadcast_UploadSchools);
                    Models.SettingsModel.Singleton.InvokeSchoolsChanged();
                    break;

                case (int)JsonCommandIDs.Broadcast_UploadTables:
                    ((JsonServer)sender).BroadcastLine((int)JsonCommandIDs.Broadcast_UploadTables);
                    Models.SettingsModel.Singleton.InvokeUpload();
                    break;

                case (int)JsonCommandIDs.GetTime:   reply = Models.SettingsModel.Singleton.InvokeGetTime();  return true;
            }


            return false;
        }
    }
}
