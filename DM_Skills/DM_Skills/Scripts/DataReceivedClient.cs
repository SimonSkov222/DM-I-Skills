using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class DataReceivedClient : IDataReceive
    {

        public bool OnData(object sender, int command, object data, out object reply)
        {
            reply = null;

            switch (command)
            {
                case (int)JsonCommandIDs.GetLocation:
                case (int)JsonCommandIDs.Broadcast_LocationChanged: Models.SettingsModel.Singleton.InvokeLocationChanged(data); break;
                case (int)JsonCommandIDs.Broadcast_TimerStarted:    Models.SettingsModel.Singleton.InvokeTimerStarted(); break;
                case (int)JsonCommandIDs.Broadcast_UploadSchools:   Models.SettingsModel.Singleton.InvokeSchoolsChanged(); break;
                case (int)JsonCommandIDs.Broadcast_UploadTables:    Models.SettingsModel.Singleton.InvokeUpload(); break;

                case (int)JsonCommandIDs.Broadcast_TimerStopped:    Models.SettingsModel.Singleton.InvokeTimerStopped(); break;
                case (int)JsonCommandIDs.Broadcast_TimerReset:      Models.SettingsModel.Singleton.InvokeTimerReset(); break;
                case (int)JsonCommandIDs.GetTime:                   Models.SettingsModel.Singleton.InvokeSetTime((double)data); break;
            }

            return false;
        }
    }
}
