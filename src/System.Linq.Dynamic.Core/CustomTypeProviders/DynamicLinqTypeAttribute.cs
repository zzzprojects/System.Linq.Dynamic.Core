namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// Indicates to Dynamic Linq to consider the Type as a valid dynamic linq type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class DynamicLinqTypeAttribute : Attribute
    {
    }
}