#if !(WINDOWS_APP || UAP10_0)
using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The default <see cref="IDynamicLinkCustomTypeProvider"/>. Scans the current AppDomain for all types marked with <see cref="DynamicLinqTypeAttribute"/>, and adds them as custom Dynamic Link types.
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : IDynamicLinkCustomTypeProvider
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
            return _customTypes ?? (_customTypes = new HashSet<Type>(FindTypesMarkedWithAttribute()));
        }

        protected IEnumerable<Type> FindTypesMarkedWithAttribute()
        {
            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
#if !NET35
            assemblies = assemblies.Where(x => !x.IsDynamic);
#endif

            var definedTypes = ExceptionFriedlyGetAssemblyTypes(assemblies);

#if (WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            return definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());
#else
            return definedTypes.Where(x => x.GetCustomAttributes(typeof(DynamicLinqTypeAttribute), false).Any());
#endif
        }

#if (WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
        protected IEnumerable<TypeInfo> ExceptionFriedlyGetAssemblyTypes(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                IEnumerable<TypeInfo> definedTypes = null;

                try
                {
                    definedTypes = assembly.DefinedTypes;
                }
                catch (Exception)
                { }

                if (definedTypes != null)
                {
                    foreach (var definedType in definedTypes)
                    {
                        yield return definedType;
                    }
                }
            }
        }
#else
        protected IEnumerable<Type> ExceptionFriedlyGetAssemblyTypes(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                IEnumerable<Type> definedTypes = null;

                try
                {
                    definedTypes = assembly.GetTypes();
                }
                catch (Exception)
                { }

                if (definedTypes != null)
                {
                    foreach (var definedType in definedTypes)
                    {
                        yield return definedType;
                    }
                }
            }
        }
#endif
    }
}
#endif