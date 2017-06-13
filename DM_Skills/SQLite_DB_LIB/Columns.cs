namespace SQLite_DB_LIB
{
    public class Column
    {
        public const string TYPE_INT = "INTEGER";
        public const string TYPE_STRING = "TEXT";
        public const string TYPE_BLOB = "BLOB";

        public string name;
        public string type;
        public string defaultValue = null;
        public string foreignKeyReferences = null;
        public bool isAutoIncrement = false;
        public bool isNotNull = false;
        public bool isPrimaryKey = false;
        public bool isUniqueKey = false;

        public string GetColumn()
        {
            string query = string.Format("`{0}` {1}", name, type);

            if (isPrimaryKey) query += " PRIMARY KEY";
            if (isUniqueKey) query += " UNIQUE";
            if (isAutoIncrement) query += " AUTOINCREMENT";
            if (isNotNull) query += " NOT NULL";

            if (defaultValue != null)
            {
                if (type.ToUpper() == Column.TYPE_INT) query += string.Format(" DEFAULT {0}", defaultValue);
                else query += string.Format(" DEFAULT '{0}'", defaultValue);
            }

            return query;
        }

        public string GetForeignKey()
        {
            if (foreignKeyReferences == null) return null;
            return string.Format("FOREIGN KEY(`{0}`) REFERENCES {1}", name, foreignKeyReferences);
        }
    }
}
