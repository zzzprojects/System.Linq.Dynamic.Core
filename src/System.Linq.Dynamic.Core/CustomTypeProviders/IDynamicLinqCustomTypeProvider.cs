using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// Interface for providing functionality to find custom types for or resolve any type.
    /// </summary>
    public interface IDynamicLinqCustomTypeProvider
    {
        /// <summary>
        /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
        /// </summary>
        /// <returns>A <see cref="HashSet{Type}" /> list of custom types.</returns>
        HashSet<Type> GetCustomTypes();

        /// <summary>
        /// Returns a list of custom extension methods that System.Linq.Dynamic.Core will understand.
        /// </summary>
        /// <returns>A list of custom extension methods that System.Linq.Dynamic.Core will understand.</returns>
        Dictionary<Type, List<MethodInfo>> GetExtensionMethods();

        /// <summary>
        /// Resolve any type by fullname which is registered in the current application domain.
        /// </summary>
        /// <param name="typeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        Type ResolveType([NotNull] string typeName);

        /// <summary>
        /// Resolve any type by the simple name which is registered in the current application domain.
        /// </summary>
        /// <param name="simpleTypeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        Type ResolveTypeBySimpleName([NotNull] string simpleTypeName);
    }
}
