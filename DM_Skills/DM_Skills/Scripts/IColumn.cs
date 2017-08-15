using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{


    public enum ColumnTypes {
        Int,
        String
    }
    interface IColumn
    {
        string DefaultValue { get; set; }
        string ForeignKeyReferences { get; set; }
        bool IsAutoIncrement { get; set; }
        bool IsNotNull { get; set; }
        bool IsPrimaryKey { get; set; }
        bool IsUniqueKey { get; set; }

        ColumnTypes Type { get; set; }
        string Name { get; set; }

    }
}
