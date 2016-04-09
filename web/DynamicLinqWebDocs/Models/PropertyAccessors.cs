using System;

namespace DynamicLinqWebDocs.Models
{
    [Flags]
    public enum PropertyAccessors
    {
        Get = 0x01,
        Set = 0x02,
        GetSet = 0x03,
    }
}