using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    public enum Order
    {
        [Description("Foo Bar")]
        FooBar,

        [Description("Bar Foo")]
            BarFoo
    }
}
