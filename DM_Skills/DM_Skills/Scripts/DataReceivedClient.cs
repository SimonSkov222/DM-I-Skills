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
                case (int)JsonCommandIDs.Broadcast_LocationChanged: Models.SettingsModel.Singleton.InvokeLocationChanged(); Console.WriteLine("Location Changed"); break;
                case (int)JsonCommandIDs.Broadcast_TimerStarted:    Models.SettingsModel.Singleton.InvokeTimerStarted(); Console.WriteLine("Location Changed"); break;
                case (int)JsonCommandIDs.Broadcast_UploadSchools:   Models.SettingsModel.Singleton.InvokeSchoolsChanged(); Console.WriteLine("Location Changed"); break;
                case (int)JsonCommandIDs.Broadcast_UploadTables:    Models.SettingsModel.Singleton.InvokeUpload(); Console.WriteLine("Location Changed"); break;
            }

            return false;
        }
    }
}
