using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Dynamic.Core.Tests
{
    public class TestCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        private HashSet<Type>? _customTypes;

        public virtual HashSet<Type> GetCustomTypes()
        {
            if (_customTypes != null)
            {
                return _customTypes;
            }

            _customTypes = new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(new[] { GetType().GetTypeInfo().Assembly }))
                {
                    typeof(CustomClassWithStaticMethod),
                    typeof(StaticHelper)
                };
            return _customTypes;
        }

        public Dictionary<Type, List<MethodInfo>> GetExtensionMethods()
        {
            var types = GetCustomTypes();

            var list = new List<Tuple<Type, MethodInfo>>();

            foreach (var type in types)
            {
                var extensionMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.IsDefined(typeof(ExtensionAttribute), false)).ToList();

                extensionMethods.ForEach(x => list.Add(new Tuple<Type, MethodInfo>(x.GetParameters()[0].ParameterType, x)));
            }

            return list.GroupBy(x => x.Item1, tuple => tuple.Item2).ToDictionary(key => key.Key, methods => methods.ToList());
        }

        public Type ResolveType(string typeName)
        {
            return Type.GetType(typeName)!;
        }

        public Type ResolveTypeBySimpleName(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return ResolveTypeBySimpleName(assemblies, typeName)!;
        }
    }
}