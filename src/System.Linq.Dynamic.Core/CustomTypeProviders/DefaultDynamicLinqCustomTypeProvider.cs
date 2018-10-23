#if !(WINDOWS_APP || UAP10_0)
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The default <see cref="IDynamicLinkCustomTypeProvider"/>.
    /// Scans the current AppDomain for all types marked with <see cref="DynamicLinqTypeAttribute"/>, and adds them as custom Dynamic Link types.
    ///
    /// Also provides functionality to resolve a Type in the current Application Domain.
    ///
    /// This class is used as default for full .NET Framework, so not for .NET Core
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        private readonly IAssemblyHelper _assemblyHelper = new DefaultAssemblyHelper();

        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.GetCustomTypes"/>
        public virtual HashSet<Type> GetCustomTypes()
        {
            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
        }

        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.ResolveType"/>
        public Type ResolveType(string typeName)
        {
            Check.NotEmpty(typeName, nameof(typeName));

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return ResolveType(assemblies, typeName);
        }
    }
}
#endif
