using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        private Dictionary<Type, List<MethodInfo>> _cachedExtensionMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDynamicLinqCustomTypeProvider"/> class.
        /// </summary>
        /// <param name="cacheCustomTypes">Defines whether to cache the CustomTypes (including extension methods) which are found in the Application Domain. Default set to 'true'.</param>
        public DefaultDynamicLinqCustomTypeProvider(bool cacheCustomTypes = true)
        {
            _cacheCustomTypes = cacheCustomTypes;
        }

        /// <inheritdoc cref="IDynamicLinqCustomTypeProvider.GetCustomTypes"/>
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

        /// <inheritdoc cref="IDynamicLinqCustomTypeProvider.GetExtensionMethods"/>
        public Dictionary<Type, List<MethodInfo>> GetExtensionMethods()
        {
            if (_cacheCustomTypes)
            {
                if (_cachedExtensionMethods == null)
                {
                    _cachedExtensionMethods = GetExtensionMethodsInternal();
                }

                return _cachedExtensionMethods;
            }

            return GetExtensionMethodsInternal();
        }

        /// <inheritdoc cref="IDynamicLinqCustomTypeProvider.ResolveType"/>
        public Type ResolveType(string typeName)
        {
            Check.NotEmpty(typeName, nameof(typeName));

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return ResolveType(assemblies, typeName);
        }

        /// <inheritdoc cref="IDynamicLinqCustomTypeProvider.ResolveTypeBySimpleName"/>
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

        private Dictionary<Type, List<MethodInfo>> GetExtensionMethodsInternal()
        {
            var types = GetCustomTypes();

            List<Tuple<Type, MethodInfo>> list = new List<Tuple<Type, MethodInfo>>();

            foreach (var type in types)
            {
                var extensionMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.IsDefined(typeof(ExtensionAttribute), false)).ToList();

                extensionMethods.ForEach(x => list.Add(new Tuple<Type, MethodInfo>(x.GetParameters()[0].ParameterType, x)));
            }

            return list.GroupBy(x => x.Item1, tuple => tuple.Item2).ToDictionary(key => key.Key, methods => methods.ToList());
        }
    }
}
