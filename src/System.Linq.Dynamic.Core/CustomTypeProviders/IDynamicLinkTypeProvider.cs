using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// Interface for providing custom types for System.Linq.Dynamic.Core.
    /// </summary>
    public interface IDynamicLinkCustomTypeProvider
    {
        /// <summary>
        /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.HashSet&lt;Type&gt;" /> list of custom types.</returns>
        HashSet<Type> GetCustomTypes();
    }
}