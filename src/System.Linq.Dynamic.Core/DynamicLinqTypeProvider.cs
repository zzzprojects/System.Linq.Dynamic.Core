using System.Collections.Generic;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Indicates to Dynamic Linq to consider the Type as a valid dynamic linq type. Use only when
    /// <see cref="GlobalConfig"/>.CustomTypeProvider is set to <see cref="DefaultDynamicLinqCustomTypeProvider"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class DynamicLinqTypeAttribute : Attribute { }

    /// <summary>
    /// The default <see cref="IDynamicLinkCustomTypeProvider"/>. Scans the current <see cref="AppDomain"/> for all types marked with 
    /// <see cref="DynamicLinqTypeAttribute"/>, and adds them as custom Dynamic Link types.
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : IDynamicLinkCustomTypeProvider
    {
        HashSet<Type> _customTypes;

        /// <summary>
        /// Returns a list of custom types that Dynamic Linq will understand.
        /// </summary>
        public virtual HashSet<Type> GetCustomTypes()
        {
            if (_customTypes == null) _customTypes = new HashSet<Type>(FindTypesMarkedWithAttribute());

            return _customTypes;
        }

        static IEnumerable<Type> FindTypesMarkedWithAttribute()
        {
#if !(NETFX_CORE || DNXCORE50 || DOTNET5_4)
            return AppDomain.CurrentDomain.GetAssemblies()
#if !(NET35)
                .Where(x => !x.IsDynamic)
#endif
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttributes(typeof(DynamicLinqTypeAttribute), false).Any());
#else
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var definedTypes = assemblies.SelectMany(x => x.DefinedTypes);
            return definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());
#endif
        }
    }
}