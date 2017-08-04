using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    interface IDatabase
    {
        void Connect(string connectionString, string prefix = "");

        void Disconnect();

        void Insert(string table, string column, object value);

        void Insert(string table, string[] columns, object[] values);

        void Insert(string table, string[] columns, List<object[]> values);

        List<object> GetRow(string table, string[] columns, string more = "");

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








    }
}
