using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class MSColumns : IColumn
    {

        public const string TYPE_INT = "int";
        public const string TYPE_VARCHAR = "varchar({0})";
        public int length = 255;
        public string name;
        public string type;
        public string defaultValue = null;
        public string foreignKeyReferences = null;
        public bool isAutoIncrement = false;
        public bool isNotNull = false;
        public bool isPrimaryKey = false;
        public bool isUniqueKey = false;


        /// <summary>
        /// Laver SQL query delen for denne column
        /// (uden foregin key)
        /// </summary>
        public string GetColumn()
        {
            type = string.Format(type, length);
            string query = string.Format("[{0}] {1}", name, type);

            if (isUniqueKey) query += " UNIQUE";
            if (isAutoIncrement) query += " IDENTITY(1,1)";
            if (isNotNull) query += " NOT NULL";

            if (defaultValue != null)
            {
                if (type.ToUpper() == TYPE_INT)
                    query += string.Format(" DEFAULT {0}", defaultValue);
                else
                    query += string.Format(" DEFAULT '{0}'", defaultValue);
            }

            if (isPrimaryKey) query += string.Format(", PRIMARY KEY([{0}])", name);

            return query;
        }

        /// <summary>
        /// Laver SQL query delen for foregin
        /// key på denne column
        /// </summary>
        public string GetForeignKey()
        {
            if (foreignKeyReferences == null) return null;
            return string.Format(" FOREIGN KEY REFERENCES {0}", foreignKeyReferences);
        }
    }
}
