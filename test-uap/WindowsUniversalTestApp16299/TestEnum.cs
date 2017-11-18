using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace WindowsUniversalTestApp16299
{
    [DynamicLinqType]
    public enum TestEnum
    {
        Var1 = 0,
        Var2 = 1,
        Var3 = 2,
        Var4 = 4,
        Var5 = 8,
        Var6 = 16,
    }
}
