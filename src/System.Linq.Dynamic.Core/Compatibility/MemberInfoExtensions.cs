#if NET35 || NET40
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    internal static class MemberInfoExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
        }
    }
}
#endif
