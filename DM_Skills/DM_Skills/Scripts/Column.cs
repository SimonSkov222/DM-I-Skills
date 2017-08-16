using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class Column : IColumn
    {
        public string DefaultValue { get; set; }
        public string ForeignKeyReferences { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsNotNull { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUniqueKey { get; set; }
        public ColumnTypes Type { get; set; }
        public string Name { get; set; }

        public Column()
        {
            DefaultValue = null;
            ForeignKeyReferences = null;
            IsAutoIncrement = false;
            IsNotNull = false;
            IsPrimaryKey = false;
            IsUniqueKey = false;
        }

        public string GetColumn()
        {
            throw new NotImplementedException();
        }

        public string GetForeignKey()
        {
            throw new NotImplementedException();
        }
    }
}
