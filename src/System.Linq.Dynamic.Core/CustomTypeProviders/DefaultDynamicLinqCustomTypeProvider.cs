#if !(WINDOWS_APP || UAP10_0)
using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The default <see cref="IDynamicLinkCustomTypeProvider"/>. Scans the current <see cref="AppDomain"/> for all types marked with 
    /// <see cref="DynamicLinqTypeAttribute"/>, and adds them as custom Dynamic Link types.
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : IDynamicLinkCustomTypeProvider
    {
        private readonly IAssemblyHelper _assemblyHelper = new DefaultAssemblyHelper();
        private HashSet<Type> _customTypes;

        /// <summary>
        /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
        /// </summary>
        public virtual HashSet<Type> GetCustomTypes()
        {
            return _customTypes ?? (_customTypes = new HashSet<Type>(FindTypesMarkedWithAttribute()));
        }

        private IEnumerable<Type> FindTypesMarkedWithAttribute()
        {
            var assemblies = _assemblyHelper.GetAssemblies();
#if !NET35
            assemblies = assemblies.Where(x => !x.IsDynamic).ToArray();
#endif

#if (WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            var definedTypes = assemblies.SelectMany(x => x.DefinedTypes);
            return definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());
#else
            var definedTypes = assemblies.SelectMany(x => x.GetTypes());
            return definedTypes.Where(x => x.GetCustomAttributes(typeof(DynamicLinqTypeAttribute), false).Any());
#endif
        }
    }
}
#endif