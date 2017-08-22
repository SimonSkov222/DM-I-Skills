using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DM_Skills.Models
{
    [Serializable]
    public abstract class ModelSettings : INotifyPropertyChanged
    {
        public Action NotifyPropertyOnAll;
        public Action<object> CallbackUpload;
        
        
        public virtual bool Exist       { get; protected set; }
        public virtual bool CanUpload   { get { return true; } }
        public virtual int ErrNo        { get; protected set; }
        public virtual string Error   { get; protected set; }
        public bool HasError { get { return ErrNo != 0; } }
        protected abstract bool OnUpload();
        //public abstract object OnGetRow();
        //public abstract object OnGetResult();

        protected static Dictionary<int, Thread> asyncDB = new Dictionary<int, Thread>();
        protected static Random randomKey = new Random(944);

        public bool Upload() {
            if (OnUpload())
            {
                CallbackUpload?.Invoke(this);
                return true;
            }

            return false;
        }

        public static void CancelThread(int? id) {
            int key = id ?? -1;
            if (asyncDB.ContainsKey(key))
            {
                if (asyncDB[key].IsAlive)
                {
                    asyncDB[key].Abort();
                }

                asyncDB.Remove(key);
            }   
        }

        protected static int GetNewThreadID() {
            int id;

            do
            {
                id = randomKey.Next(0, 2000);
            } while (asyncDB.ContainsKey(id));
            
            return id;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static void RequestBroadcast(Scripts.PacketType type)
        {
            SettingsModel Settings = Application.Current.FindResource("Settings") as SettingsModel;
            if (Settings.IsServer)
            {
                Settings.Server.Broadcast(type);
            }
            else if (Settings.IsClient)
            {
                Settings.Client.Broadcast(type);
            }
        }

        //public static object GetRow<T>(int id) { return OnGetRow(); }
        //public static object GetResults<T>(int limit, int offset) { }
    }
}
