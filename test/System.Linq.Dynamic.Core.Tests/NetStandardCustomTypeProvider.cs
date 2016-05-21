using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Xunit.Runner.DotNet;

namespace System.Linq.Dynamic.Core.Tests
{
    class NetStandardCustomTypeProvider : IDynamicLinkCustomTypeProvider
    {
        public HashSet<Type> GetCustomTypes()
        {
            var thisType = typeof (Program);
            var assemblies = System.AppDomain.NetCoreApp.AppDomain.CurrentDomain.GetAssemblies(thisType).Where(x => !x.IsDynamic).ToArray();

            var definedTypes = assemblies.SelectMany(x => x.DefinedTypes);
            var types = definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());

            return new HashSet<Type>(types);
        }
    }
}