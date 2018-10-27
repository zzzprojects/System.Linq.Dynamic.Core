using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace System.Linq.Dynamic.Core.Tests
{
    class NetStandardCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        public HashSet<Type> GetCustomTypes()
        {
            var thisType = GetType();

#if NETCORE1_1
            var assemblies = AppDomain.NetCoreApp.AppDomain.CurrentDomain.GetAssemblies(thisType).Where(x => !x.IsDynamic).ToArray();
#else
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();
#endif
            return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
        }

        public Type ResolveType(string typeName)
        {
            var thisType = GetType();

#if NETCORE1_1
            var assemblies = AppDomain.NetCoreApp.AppDomain.CurrentDomain.GetAssemblies(thisType).ToArray();
#else
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();
#endif
            return ResolveType(assemblies, typeName);
        }
    }
}
