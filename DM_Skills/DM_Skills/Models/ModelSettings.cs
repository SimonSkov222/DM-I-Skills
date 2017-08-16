using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public bool Upload() {
            if (OnUpload())
            {
                CallbackUpload?.Invoke(this);
                return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        //public static object GetRow<T>(int id) { return OnGetRow(); }
        //public static object GetResults<T>(int limit, int offset) { }
    }
}
