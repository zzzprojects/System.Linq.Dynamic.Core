using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The default implementation for <see cref="IDynamicLinkCustomTypeProvider"/>.
    /// 
    /// Scans the current AppDomain for all types marked with <see cref="DynamicLinqTypeAttribute"/>, and adds them as custom Dynamic Link types.
    ///
    /// Also provides functionality to resolve a Type in the current Application Domain.
    ///
    /// This class is used as default for full .NET Framework, so not for .NET Core
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        private readonly IAssemblyHelper _assemblyHelper = new DefaultAssemblyHelper();
        private readonly bool _cacheCustomTypes;

        private HashSet<Type> _cachedCustomTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDynamicLinqCustomTypeProvider"/> class.
        /// </summary>
        /// <param name="cacheCustomTypes">Defines whether to cache the CustomTypes which are found in the Application Domain. Default set to 'true'.</param>
        public DefaultDynamicLinqCustomTypeProvider(bool cacheCustomTypes = true)
        {
            _cacheCustomTypes = cacheCustomTypes;
        }

        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.GetCustomTypes"/>
        public virtual HashSet<Type> GetCustomTypes()
        {
            if (_cacheCustomTypes)
            {
                if (_cachedCustomTypes == null)
                {
                    _cachedCustomTypes = GetCustomTypesInternal();
                }

                return _cachedCustomTypes;
            }

            return GetCustomTypesInternal();
        }

        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.ResolveType"/>
        public Type ResolveType(string typeName)
        {
            Check.NotEmpty(typeName, nameof(typeName));

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return ResolveType(assemblies, typeName);
        }

        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.ResolveTypeBySimpleName"/>
        public Type ResolveTypeBySimpleName(string simpleTypeName)
        {
            Check.NotEmpty(simpleTypeName, nameof(simpleTypeName));

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return ResolveTypeBySimpleName(assemblies, simpleTypeName);
        }

        private HashSet<Type> GetCustomTypesInternal()
        {
            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
        }
    }
}
