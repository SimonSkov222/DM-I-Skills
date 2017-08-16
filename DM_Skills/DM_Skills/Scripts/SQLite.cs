using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class SQLite : IDatabase
    {
        private string _prefix;
        public string Prefix { get { return _prefix; } }

        private bool _isConnected = false;
        public bool IsConnected { get { return _isConnected; } }

        private string _lastQuery;
        public string LastQuery { get { return _lastQuery; } }

        public bool UseDistinct { get; set; }

        private SQLiteConnection sql_conn;
        private SQLiteCommand sql_cmd;


        /// <summary>
        /// Opret forbindelse til databasen
        /// </summary>
        public void Connect(string connectionString, string prefix = "")
        {
            //Start connection
            sql_conn = new SQLiteConnection(connectionString);
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();

            _prefix = prefix;
            _isConnected = true;
        }

        /// <summary>
        /// Afbryd forbindelse til databasen
        /// </summary>
        public void Disconnect()
        {
            sql_cmd.Dispose(); //release all resouces
            sql_conn.Close();
            _isConnected = false;
        }

        public object Insert(string table, string[] columns, List<object[]> values)
        {
            List<object> pKeys = new List<object>();
            foreach (var item in values)
            {
                pKeys.Add(Insert(table, columns, item));

            }
            return pKeys;
        }

        /// <summary>
        /// Indsæt en row med en enkelt værdi til tabel
        /// </summary>
        public object Insert(string table, string column, object value)
        {
            return Insert(table, new string[] { column }, new object[] { value });
        }

        /// <summary>
        /// Indsæt en row med flere værdier til tabel
        /// </summary>
        public object Insert(string table, string[] columns, object[] values)
        {
            string tableWithPrefix = Prefix + table;
            values = StringsSQLready(values);

            string columnsCMD = string.Format("`{0}`", string.Join("`, `", columns.ToArray()));
            string valuesCMD = string.Join(", ", values.ToArray());

            ExecuteQuery(string.Format("INSERT INTO `{0}`({1}) VALUES({2});", tableWithPrefix, columnsCMD, valuesCMD));


            string pColumn = GetPrimaryKeyName(table);

            if (pColumn != "")
            {

                List<string> whrArr = new List<string>();
                for (int i = 0; i < columns.Length; i++)
                    whrArr.Add(string.Format("`{0}` = {1}", columns[i], values[i]));

                string whrCMD = "WHERE " + string.Join(" AND ", whrArr.ToArray());


                var result = GetRow(table, pColumn, whrCMD);
                if (result.Count > 0) return result[0];
            }


            return false;
        }

        public List<object> GetRow(string table, string column, string format = "", params object[] arg)
        {
            return GetRow(table, new string[] { column }, format, arg);
        }

        /// <summary>
        /// Hent den første row her kan man bestemme om 
        /// man vil bruge column navn eller den position
        /// den har i columns som key for at finde 
        /// frem til værdien
        /// </summary>
        public List<object> GetRow(string table, string[] column, string format = "", params object[] arg)
        {
            string query = BuildGetRowsCMD(table, column, string.Format(format, arg));

            var result = ExecuteQuery(query);

            if (result.Count == 0) return null;
            else return result[0];

        }

        public List<List<object>> GetRows(string table, string column, string format = "", params object[] arg)
        {
            return GetRows(table, new string[] { column }, format, arg);
        }

        /// <summary>
        /// Hent flere rows her kan man bestemme om 
        /// man vil bruge column navn eller den position
        /// den har i columns som key for at finde 
        /// frem til værdien
        /// </summary>
        public List<List<object>> GetRows(string table, string[] columns, string format = "", params object[] arg)
        {
            string query = BuildGetRowsCMD(table, columns, string.Format(format, arg));

            var result = ExecuteQuery(query);

            if (result.Count == 0) return null;
            else return result;
        }

        /// <summary>
        /// Opret en tabel i databasen
        /// </summary>
        public void Create(string table, params IColumn[] columns)
        {
            string tableWithPrefix = Prefix + table;

            List<string> columnCMD = new List<string>();
            List<string> foreignCMD = new List<string>();


            foreach (var column in columns)
            {

                columnCMD.Add(GetColumn(column));
                
                if (GetForeignKey(column) != null)
                    foreignCMD.Add(GetForeignKey(column));

            }

            string columnPart = string.Join(",\n ", columnCMD.ToArray());
            string foreignKeyPart = foreignCMD.Count > 0 ? ",\n " + string.Join(",\n ", foreignCMD.ToArray()) : "";

            string command = string.Format("CREATE TABLE `{0}`\n(\n {1}{2}\n);", tableWithPrefix, columnPart, foreignKeyPart);
            ExecuteQuery(command);
        }

        /// <summary>
        /// Slet en tabel i databasen
        /// </summary>
        public void Drop(string table)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("DROP TABLE `{0}`;", tableWithPrefix);
            ExecuteQuery(command);
        }

        /// <summary>
        /// Tjek om en tabel findes i databasen
        /// </summary>
        public bool Exist(string table)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("SELECT `name` FROM `sqlite_master` WHERE `type`='table' AND `name`='{0}';", tableWithPrefix);
            var result = ExecuteQuery(command);

            return result.Count > 0;
        }

        /// <summary>
        /// Tjek om en tabel har en row med disse værdier
        /// </summary>
        public bool Exist(string table, string[] columns, object[] values)
        {
            string tableWithPrefix = Prefix + table;

            List<string> statements = new List<string>();

            values = StringsSQLready(values);


            for (int i = 0; i < columns.Length; i++)
                statements.Add(string.Format("`{0}` = {1}", columns[i], values[i]));


            string statementsCMD = string.Join(" AND ", statements.ToArray());
            string command = string.Format("SELECT * FROM `{0}` WHERE {1};", tableWithPrefix, statementsCMD);
            var result = ExecuteQuery(command);

            return result.Count > 0;
        }

        /// <summary>
        /// Tjek om en tabel har en row med denne værdi
        /// </summary>
        public bool Exist(string table, string column, object value)
        {
            return Exist(table, new string[] { column }, new object[] { value });
        }

        /// <summary>
        /// Kan udføre en selv skrevet SQL query/kommando
        /// og hvis det er en select kan man selv 
        /// vælge man bruge columns eller position som key
        /// </summary>
        public List<List<object>> ExecuteQuery(string cmd)
        {
            SQLiteDataReader sql_reader;
            var result = new List<List<object>>();
            bool isRead = false;
            //Metoder der vil hente data
            string[] readMethods = { "SELECT", "PRAGMA" };

            //Find ud af om vi skal hente data
            foreach (var item in readMethods)
            {
                if (cmd.ToUpper().StartsWith(item))
                {
                    isRead = true;
                    break;
                }
            }

            //SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter();

            _lastQuery = cmd;
            sql_cmd.CommandText = cmd;

            //udfør kommando hvor man ikke venter på data
            if (!isRead)
            {
                sql_cmd.ExecuteNonQuery();
                return null;
            }

            //Udfør kommando hvor man skal have data tilbage
            sql_reader = sql_cmd.ExecuteReader();
            while (sql_reader.Read())
            {
                var row = new List<object>();

                for (int i = 0; i < sql_reader.FieldCount; i++)
                {
                    row.Add(sql_reader[i]);
                }
                result.Add(row);
            }
            sql_reader.Close();
            return result;
        }

        ///// <summary>
        ///// Kan udføre en selv skrevet SQL query/kommando
        ///// og hvis det er en select vil position være key
        ///// </summary>
        //public List<Dictionary<int, object>> ExecuteQuery(string cmd) { return ExecuteQuery<int>(cmd); }

        /// <summary>
        /// Hent primary key column navn fra en tabel
        /// </summary>
        public string GetPrimaryKeyName(string table)
        {

            string tableWithPrefix = Prefix + table;
            var result = ExecuteQuery(string.Format("PRAGMA table_info(`{0}`)", tableWithPrefix));

            for (int i = 0; i < result.Count; i++)
            {
                if (Convert.ToBoolean(result[i][5])) return (string)result[i][1];
            }

            return null;
        }

        /// <summary>
        /// Fjerner ' fra strings
        /// </summary>
        public string EscapeString(string value) { return value.Replace("'", ""); }

        /// <summary>
        /// Sætter object som er string i ''
        /// </summary>
        public object[] StringsSQLready(object[] values)
        {

            for (int i = 0; i < values.Length; i++)
                if (values[i] is string)
                    values[i] = string.Format("'{0}'", values[i]);

            return values;
        }


        /// <summary>
        /// Bygger SQL for SELECT query
        /// </summary>
        public string BuildGetRowsCMD(string table, string[] columns, string more = "")
        {

            string[] sqlMethod = { "*", "COUNT" };

            string tableWithPrefix = Prefix + table;


            for (int i = 0; i < columns.Length; i++)
            {

                bool isMethod = false;
                foreach (string method in sqlMethod)
                {
                    if (columns[i].ToUpper().StartsWith(method))
                    {
                        isMethod = true;
                        break;
                    }
                }

                if (isMethod) continue;
                if (columns[i].StartsWith("`") && columns[i].EndsWith("`")) continue;
                columns[i] = "`" + columns[i] + "`";
            }

            return string.Format("SELECT {1} FROM `{0}`{2};", tableWithPrefix, string.Join(", ", columns), " " + more);
        }

        public string GetTableName(string name)
        {
            return Prefix + name;
        }

        public string GetColumn(IColumn col)
        {

            string type = "";

            if (col.Type == ColumnTypes.Int)
            {
                type = "INTEGER";
            }
            else if (col.Type == ColumnTypes.String)
            {
                type = "STRING";
            }

            string query = string.Format("`{0}` {1}", col.Name, type);

            if (col.IsPrimaryKey) query += " PRIMARY KEY";
            if (col.IsUniqueKey) query += " UNIQUE";
            if (col.IsAutoIncrement) query += " AUTOINCREMENT";
            if (col.IsNotNull) query += " NOT NULL";

            if (col.DefaultValue != null)
            {
                if (col.Type == ColumnTypes.Int) query += string.Format(" DEFAULT {0}", col.DefaultValue);
                else query += string.Format(" DEFAULT '{0}'", col.DefaultValue);
            }

            return query;
        }

        public string GetForeignKey(IColumn col)
        {
            if (col.ForeignKeyReferences == null) return null;
            return string.Format("FOREIGN KEY(`{0}`) REFERENCES {1}", col.Name, col.ForeignKeyReferences);
        }
    }
}
