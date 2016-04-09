using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Interface for providing custom types for Dynamic Linq.
    /// </summary>
    public interface IDynamicLinkCustomTypeProvider
    {
        /// <summary>
        /// Returns a list of custom types that Dynamic Linq will understand.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        HashSet<Type> GetCustomTypes();
    }
}
