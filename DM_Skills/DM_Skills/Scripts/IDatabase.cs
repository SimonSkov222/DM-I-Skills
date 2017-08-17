using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    interface IDatabase
    {
        bool IsConnected { get; }
        string LastQuery { get; }
        Action<object> CallBack { get; set; }

        bool UseDistinct { get; set; }

        void Connect(string connectionString, string prefix = "");

        void Disconnect();
        
        object Insert(string table, string column, object value);

        object Insert(string table, string[] columns, object[] values);

        object Insert(string table, string[] columns, List<object[]> values);

        List<object> GetRow(string table, string columns, string format = "", params object[] arg);

        List<object> GetRow(string table, string[] columns, string format = "", params object[] arg);

        List<List<object>> GetRows(string table, string columns, string format = "", params object[] arg);

        List<List<object>> GetRows(string table, string[] columns, string format = "", params object[] arg);

        void Create(string table, params IColumn[] columns);

        void Drop(string table);

        bool Exist(string table);

        bool Exist(string table, string[] columns, object[] values);

        bool Exist(string table, string column, object value);

        List<List<object>> ExecuteQuery(string cmd);

        string GetPrimaryKeyName(string table);

        string EscapeString(string value);

        object[] StringsSQLready(object[] values);

        string BuildGetRowsCMD(string table, string[] columns, string more = "");

        string GetTableName(string name);

        string GetColumn(IColumn col);

        string GetForeignKey(IColumn col);

        void Update(string table, string column, object value, string format = "", params object[] arg);

        void Update(string table, string[] columns, object[] values, string format = "", params object[] arg);

        void Update(string table, string[] columns, object[] values, object pValue);

        void Update(string table, string column, object value, object pValue);

        void Delete(string table, string format, params object[] arg);









    }
}
