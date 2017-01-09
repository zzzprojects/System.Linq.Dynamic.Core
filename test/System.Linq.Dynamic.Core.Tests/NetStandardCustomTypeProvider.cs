using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace System.Linq.Dynamic.Core.Tests
{
    class NetStandardCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        public HashSet<Type> GetCustomTypes()
        {
            var thisType = typeof (Program);
            var assemblies = AppDomain.NetCoreApp.AppDomain.CurrentDomain.GetAssemblies(thisType).Where(x => !x.IsDynamic).ToArray();

            return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
        }
    }
}