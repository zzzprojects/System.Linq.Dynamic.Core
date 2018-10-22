using System.Collections.Generic;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// Interface for providing functionality to find custom types for or resolve any type.
    /// </summary>
    public interface IDynamicLinkCustomTypeProvider
    {
        /// <summary>
        /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
        /// </summary>
        /// <returns>A <see cref="HashSet&lt;Type&gt;" /> list of custom types.</returns>
        HashSet<Type> GetCustomTypes();

        /// <summary>
        /// Resolve any type which is registered in the current application domain.
        /// </summary>
        /// <param name="typeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        Type ResolveType([NotNull] string typeName);

    }
}
