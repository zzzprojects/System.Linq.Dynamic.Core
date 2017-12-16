#if !(WINDOWS_APP || UAP10_0)
using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The default <see cref="IDynamicLinkCustomTypeProvider"/>. Scans the current AppDomain for all types marked with <see cref="DynamicLinqTypeAttribute"/>, and adds them as custom Dynamic Link types.
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        private readonly IAssemblyHelper _assemblyHelper = new DefaultAssemblyHelper();
        private HashSet<Type> _customTypes;

        /// <summary>
        /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.HashSet&lt;Type&gt;" /> list of custom types.
        /// </returns>
        public virtual HashSet<Type> GetCustomTypes()
        {
            if (_customTypes != null)
            {
                return _customTypes;
            }

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            _customTypes = new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
            return _customTypes;
        }
    }
}
#endif
