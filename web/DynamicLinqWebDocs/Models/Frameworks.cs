using System;
using System.ComponentModel;

namespace DynamicLinqWebDocs.Models
{
    [Flags]
    public enum Frameworks
    {
        NotSet = 0x0000,

        [Description("3.5")]
        Net35 = 0x0001,

        [Description("4.0")]
        Net40 = 0x0002,

        [Description("4.5")]
        Net45 = 0x0004,

        [Description("4.0, 4.5+")]
        Net40Plus = 0x0006,

        [Description("3.5, 4.0, 4.5+")]
        All = 0x0007,
    }
}