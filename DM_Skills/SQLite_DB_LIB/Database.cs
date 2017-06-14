using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using SQLite_LIB = System.Data.SQLite;

namespace SQLite_DB_LIB
{

    public class Database
    {
        private static string _prefix;
        public static string Prefix { get { return _prefix; } }

        private static bool _isConnected = false;
        public static bool IsConnected { get { return _isConnected; } }

        private static string _lastQuery;
        public static string LastQuery { get { return _lastQuery; } }

        private static SQLite_LIB.SQLiteConnection sql_conn;
        private static SQLite_LIB.SQLiteCommand sql_cmd;

        /// <summary>
        /// Opret forbindelse til databasen
        /// </summary>
        public static bool Connect(string connectionString, string prefix = "")
        {

            if (IsConnected) return false;

            //Start connection
            sql_conn = new SQLite_LIB.SQLiteConnection(connectionString);
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();

            _prefix = prefix;
            _isConnected = true;

            return true;
        }

        /// <summary>
        /// Afbryd forbindelse til databasen
        /// </summary>
        public static bool Disconnect()
        {

            if (!IsConnected) return false;
            sql_cmd.Dispose(); //release all resouces
            sql_conn.Close();
            _isConnected = false;

            return true;
        }

        /// <summary>
        /// Indsæt en row med en enkelt værdi til tabel
        /// </summary>
        public static object Insert(string table, string column, object value)
        {
            return Insert(table, new string[] { column }, new object[] { value });
        }

        /// <summary>
        /// Indsæt en row med flere værdier til tabel
        /// </summary>
        public static object Insert(string table, string[] columns, object[] values)
        {
            string tableWithPrefix = Prefix + table;
            values = CleanValues(values);

            string columnsCMD = string.Format("`{0}`", string.Join("`, `", columns.ToArray()));
            string valuesCMD = string.Join(", ", values.ToArray());

            CustomQuery(string.Format("INSERT INTO `{0}`({1}) VALUES({2});", tableWithPrefix, columnsCMD, valuesCMD));


            string pColumn = GetPrimaryKeyName(table);

            if (pColumn != "")
            {

                List<string> whrArr = new List<string>();
                for (int i = 0; i < columns.Length; i++)
                    whrArr.Add(string.Format("`{0}` = {1}", columns[i], values[i]));

                string whrCMD = "WHERE " + string.Join(" AND ", whrArr.ToArray());


                var result = GetRow<int>(table, new string[] { pColumn }, whrCMD);
                if (result.ContainsKey(0)) return result[0];
            }


            return false;
        }

        /// <summary>
        /// Opdater en row med flere værdier
        /// </summary>
        public static void Update(string table, string[] columns, object[] newValues, params object[] primaryKeyVal)
        {
            string tableWithPrefix = Prefix + table;
            List<string> statements = new List<string>();


            newValues = CleanValues(newValues);
            primaryKeyVal = CleanValues(primaryKeyVal);

            for (int i = 0; i < columns.Length; i++)
                statements.Add(string.Format("`{0}` = {1}", columns[i], newValues[i]));


            string pColumn = GetPrimaryKeyName(table);
            string pVals = string.Join(", ", primaryKeyVal);
            string statementsCMD = string.Join(", ", statements.ToArray());

            CustomQuery(string.Format("UPDATE `{0}` SET {1} WHERE `{2}` IN ({3});", tableWithPrefix, statementsCMD, pColumn, pVals));
        }

        /// <summary>
        /// Opdater en row med en enkelt værdi
        /// </summary>
        public static void Update(string table, string column, object newValue, params object[] primaryKeyVal)
        {
            Update(table, new string[] { column }, new object[] { newValue }, primaryKeyVal);
        }

        /// <summary>
        /// Slet en eller flere rows fra tabel
        /// </summary>
        public static void Delete(string table, params object[] primaryKeyVal)
        {
            string tableWithPrefix = Prefix + table;

            primaryKeyVal = CleanValues(primaryKeyVal);

            string pColumn = GetPrimaryKeyName(table);
            string pVals = string.Join(", ", primaryKeyVal);

            CustomQuery(string.Format("DELETE FROM `{0}` WHERE `{1}` IN ({2});", tableWithPrefix, pColumn, pVals));
        }

        /// <summary>
        /// Hent den første row her kan man bestemme om 
        /// man vil bruge column navn eller den position
        /// den har i columns som key for at finde 
        /// frem til værdien
        /// </summary>
        public static Dictionary<T, object> GetRow<T>(string table, string[] columns, string more = "")
        {
            string query = BuildGetRowsCMD(table, columns, more);

            var result = CustomQuery<T>(query);

            if (result.Count == 0) return null;
            else return result[0];
        }


        /// <summary>
        /// Hent flere rows her kan man bestemme om 
        /// man vil bruge column navn eller den position
        /// den har i columns som key for at finde 
        /// frem til værdien
        /// </summary>
        public static List<Dictionary<T, object>> GetRows<T>(string table, string[] columns, string more = "")
        {
            string query = BuildGetRowsCMD(table, columns, more);

            var result = CustomQuery<T>(query);

            if (result.Count == 0) return null;
            else return result;
        }

        /// <summary>
        /// Opret en tabel i databasen
        /// </summary>
        public static void Create(string table, params Column[] columns)
        {
            string tableWithPrefix = Prefix + table;

            List<string> columnCMD = new List<string>();
            List<string> foreignCMD = new List<string>();


            foreach (var column in columns)
            {

                columnCMD.Add(column.GetColumn());

                if (column.foreignKeyReferences != null)
                    foreignCMD.Add(column.GetForeignKey());

            }

            string columnPart = string.Join(",\n ", columnCMD.ToArray());
            string foreignKeyPart = foreignCMD.Count > 0 ? ",\n " + string.Join(",\n ", foreignCMD.ToArray()) : "";

            string command = string.Format("CREATE TABLE `{0}`\n(\n {1}{2}\n);", tableWithPrefix, columnPart, foreignKeyPart);
            CustomQuery(command);
        }

        /// <summary>
        /// Slet en tabel i databasen
        /// </summary>
        public static void Drop(string table)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("DROP TABLE `{0}`;", tableWithPrefix);
            CustomQuery(command);
        }

        /// <summary>
        /// Tjek om en tabel findes i databasen
        /// </summary>
        public static bool Exist(string table)
        {
            string tableWithPrefix = Prefix + table;

            string command = string.Format("SELECT `name` FROM `sqlite_master` WHERE `type`='table' AND `name`='{0}';", tableWithPrefix);
            var result = CustomQuery(command);

            return result.Count > 0;
        }

        /// <summary>
        /// Tjek om en tabel har en row med disse værdier
        /// </summary>
        public static bool Exist(string table, string[] columns, object[] values)
        {
            string tableWithPrefix = Prefix + table;

            List<string> statements = new List<string>();

            values = CleanValues(values);



            for (int i = 0; i < columns.Length; i++)
                statements.Add(string.Format("`{0}` = {1}", columns[i], values[i]));


            string statementsCMD = string.Join(" AND ", statements.ToArray());
            string command = string.Format("SELECT * FROM `{0}` WHERE {1};", tableWithPrefix, statementsCMD);
            var result = CustomQuery(command);

            return result.Count > 0;
        }

        /// <summary>
        /// Tjek om en tabel har en row med denne værdi
        /// </summary>
        public static bool Exist(string table, string column, object value)
        {
            return Exist(table, new string[] { column }, new object[] { value });
        }

        /// <summary>
        /// Tjekker om en bestemt row har en bestemt værdi
        /// </summary>
        public static bool IsEquals(string table, string column, object value, object primaryKeyVal)
        {
            return IsEquals(table, new string[] { column }, new object[] { value }, primaryKeyVal);
        }

        /// <summary>
        /// Tjekker om en bestemt row har flere bestemte værdier
        /// </summary>
        public static bool IsEquals(string table, string[] columns, object[] values, object primaryKeyVal)
        {

            string tableWithPrefix = Prefix + table;

            string pKey = GetPrimaryKeyName(table);

            values = CleanValues(values);

            string whrEnd = "";
            for (int i = 0; i < columns.Length; i++)
            {
                whrEnd += string.Format(" AND `{0}` = {1}", columns[i], values[i]);
            }

            string command = string.Format("SELECT * FROM `{0}` WHERE `{1}` = {2}{3};", tableWithPrefix, pKey, primaryKeyVal, whrEnd);
            var result = CustomQuery(command);

            if (result == null) return false;
            else if (result.Count == 1) return true;
            else return false;
        }

        /// <summary>
        /// Kan udføre en selv skrevet SQL query/kommando
        /// og hvis det er en select kan man selv 
        /// vælge man bruge columns eller position som key
        /// </summary>
        public static List<Dictionary<T, object>> CustomQuery<T>(string cmd)
        {
            SQLite_LIB.SQLiteDataReader sql_reader;
            var result = new List<Dictionary<T, object>>();
            string[] readMethods = { "SELECT", "PRAGMA" };
            bool isRead = false;

            foreach (var item in readMethods)
            {
                if (cmd.ToUpper().StartsWith(item))
                {
                    isRead = true;
                    break;
                }
            }

            SQLite_LIB.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter();

            _lastQuery = cmd;
            sql_cmd.CommandText = cmd;

            if (!isRead)
            {
                sql_cmd.ExecuteNonQuery();
                return null;
            }

            sql_reader = sql_cmd.ExecuteReader();
            while (sql_reader.Read())
            {
                var row = new Dictionary<T, object>();

                for (int i = 0; i < sql_reader.FieldCount; i++)
                {

                    object key;
                    if (typeof(T) == typeof(String)) key = sql_reader.GetName(i);
                    else key = i;

                    row.Add((T)key, sql_reader[i]);
                }
                result.Add(row);
            }
            sql_reader.Close();
            return result;
        }

        /// <summary>
        /// Kan udføre en selv skrevet SQL query/kommando
        /// og hvis det er en select vil position være key
        /// </summary>
        public static List<Dictionary<int, object>> CustomQuery(string cmd) { return CustomQuery<int>(cmd); }

        /// <summary>
        /// Laver en sql backup fil med med tabel sql query
        /// samt data
        /// </summary>
        public static void Backup(string filename)
        {
            SQLite_LIB.SQLiteDataReader sql_reader;
            StreamWriter fileWriter = new StreamWriter(filename);
            List<string> tableNames = new List<string>();
            string[] ignoreTables = { "sqlite_sequence" };


            fileWriter.WriteLine("/*");
            fileWriter.WriteLine(" * Creating Tables");
            fileWriter.WriteLine(" */\n");


            sql_cmd.CommandText = "SELECT `name`, `sql` FROM `sqlite_master` WHERE `type` = 'table';";
            sql_reader = sql_cmd.ExecuteReader();

            while (sql_reader.Read())
            {

                string name = sql_reader.GetString(0);
                string cmd = sql_reader.GetString(1) + ";";


                if (ignoreTables.Contains<string>(name)) continue;

                tableNames.Add(name);

                fileWriter.WriteLine(cmd);
                fileWriter.Write("\n");
            }
            sql_reader.Close();

            foreach (string table in tableNames)
            {

                sql_cmd.CommandText = "SELECT * FROM `" + table + "`;";
                sql_reader = sql_cmd.ExecuteReader();

                while (sql_reader.Read())
                {

                    List<string> values = new List<string>();

                    for (int i = 0; i < sql_reader.FieldCount; i++)
                    {
                        if (sql_reader.GetDataTypeName(i) == Column.TYPE_INT) values.Add(sql_reader[i].ToString());
                        else values.Add(string.Format("'{0}'", sql_reader[i].ToString()));
                    }

                    fileWriter.WriteLine(string.Format("INSERT INTO `{0}` VALUES({1});", table, string.Join(", ", values.ToArray())));
                }
                fileWriter.Write("\n");
                sql_reader.Close();
            }
            fileWriter.Close();
        }

        /// <summary>
        /// Læser en sql kommando fil igennem og udføre kommandoerne
        /// </summary>
        public static void Load(string filename)
        {
            string pComment = "(?:\\/\\*.+?\\*\\/|--.*?(?:\n|$))";
            string pString = @"'.*?(?<!\\)'";

            string[] sqlCMD = { "CREATE", "DROP", "SELECT", "INSERT", "UPDATE", "UPDATE", "DELETE" };
            string pQueries = string.Format("(?:{0}).+?;", string.Join("|", sqlCMD));

            RegexOptions rOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            string content = File.ReadAllText(filename);

            content = Regex.Replace(content, pComment, "", rOptions);

            MatchCollection mcStrings = Regex.Matches(content, pString, rOptions);
            for (int i = 0; i < mcStrings.Count; i++)
            {
                content = content.Replace(mcStrings[i].Value, "'" + i + "'");
            }

            MatchCollection mcQueries = Regex.Matches(content, pQueries, rOptions);

            List<string> conQueries = new List<string>();
            foreach (Match mQuery in mcQueries)
            {
                string query = mQuery.Value;
                MatchCollection mcCstrings = Regex.Matches(query, "'(.+?)'");

                foreach (Match mString in mcCstrings)
                {

                    Regex pattern = new Regex(@"(?<!\\)" + mString.Value);

                    int index = int.Parse(mString.Groups[1].Value);
                    query = pattern.Replace(query, mcStrings[index].Value, 1);
                }

                conQueries.Add(query);
            }

            foreach (var Query in conQueries)
            {
                CustomQuery(Query);
            }
        }

        /// <summary>
        /// Hent primary key column navn fra en tabel
        /// </summary>
        public static string GetPrimaryKeyName(string table)
        {

            string tableWithPrefix = Prefix + table;
            var result = CustomQuery(string.Format("PRAGMA table_info(`{0}`)", tableWithPrefix));

            for (int i = 0; i < result.Count; i++)
            {
                if (Convert.ToBoolean(result[i][5])) return (string)result[i][1];
            }

            return null;
        }

        /// <summary>
        /// Fjerner ' fra strings
        /// </summary>
        public static string EscapeString(string value) { return value.Replace("'", ""); }

        /// <summary>
        /// Sætter object som er string i ''
        /// </summary>
        private static object[] CleanValues(object[] values)
        {

            for (int i = 0; i < values.Length; i++)
                if (!values[i].IsInt())
                    values[i] = string.Format("'{0}'", values[i]);

            return values;
        }


        /// <summary>
        /// Bygger SQL Select query
        /// </summary>
        private static string BuildGetRowsCMD(string table, string[] columns, string more = "")
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
    }
}
