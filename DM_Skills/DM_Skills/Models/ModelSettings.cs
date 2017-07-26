using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    public abstract class ModelSettings
    {

        public event Action<object> CallbackUpload;
        
        public virtual bool Exist       { get; protected set; }
        public virtual bool CanUpload   { get { return true; } }
        public virtual int ErrNo        { get; protected set; }
        public virtual string Error   { get; protected set; }

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

        //public static object GetRow<T>(int id) { return OnGetRow(); }
        //public static object GetResults<T>(int limit, int offset) { }
    }
}
