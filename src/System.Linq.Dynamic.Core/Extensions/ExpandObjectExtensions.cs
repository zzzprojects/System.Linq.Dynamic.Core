//#if !(NET35 || UAP10_0)
//using System.Dynamic;
//using System.Reflection;
//using JetBrains.Annotations;

//namespace System.Linq.Dynamic.Core.Extensions
//{
//    public static class ExpandObjectExtensions
//    {
//        public static DynamicClass ToDynamicClass([NotNull] this object obj, bool createParameterCtor = true)
//        {
//            var propertyInfo = obj.GetType().GetProperties();
//            var dynamicProperties = propertyInfo.Select(p => new DynamicProperty(p.Name, p.PropertyType)).ToList();

//            Type type = DynamicClassFactory.CreateType(dynamicProperties, createParameterCtor);
//            var dynamicClass = (DynamicClass) Activator.CreateInstance(type);

//            foreach (var kvp in obj)
//                dynamicClass.SetDynamicPropertyValue(kvp.Key, kvp.Value);

//            return dynamicClass;
//        }
//    }
//}
//#endif