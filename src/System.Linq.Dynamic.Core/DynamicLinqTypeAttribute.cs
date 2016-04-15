
namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Indicates to Dynamic Linq to consider the Type as a valid dynamic linq type. Use only when
    /// <see cref="GlobalConfig"/>.CustomTypeProvider is set to <see cref="DefaultDynamicLinqCustomTypeProvider"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class DynamicLinqTypeAttribute : Attribute
    {
    }
}