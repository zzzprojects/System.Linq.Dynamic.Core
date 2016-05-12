using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// Interface for providing custom types for Dynamic Linq.
    /// </summary>
    public interface IDynamicLinkCustomTypeProvider
    {
        /// <summary>
        /// Returns a list of custom types that Dynamic Linq will understand.
        /// </summary>
        HashSet<Type> GetCustomTypes();
    }
}