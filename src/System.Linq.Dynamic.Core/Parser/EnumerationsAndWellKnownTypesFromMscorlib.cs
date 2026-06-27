using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace System.Linq.Dynamic.Core.Parser;

internal static class EnumerationsAndWellKnownTypesFromMscorlib
{
    private readonly static string SystemPrivateCoreLib = typeof(StringComparer).GetTypeInfo().Assembly.FullName!;
    private readonly static string SystemPrivateUri = typeof(UriFormat).GetTypeInfo().Assembly.FullName!;
    private readonly static string SystemPrivateXml = typeof(XmlNodeType).GetTypeInfo().Assembly.FullName!;
    private readonly static string SystemPrivateXmlLinq = typeof(XObject).GetTypeInfo().Assembly.FullName!;

    /// <summary>
    /// Enum types and well-known types.
    /// </summary>
    public static readonly ConcurrentDictionary<string, Type> PredefinedEnumerationTypes = new(StringComparer.OrdinalIgnoreCase);

    static EnumerationsAndWellKnownTypesFromMscorlib()
    {
        var list = AddEnumsAndWellKnownTypesFromAssembly(SystemPrivateUri);
        list.AddRange(AddEnumsAndWellKnownTypesFromAssembly(SystemPrivateCoreLib));
        list.AddRange(AddEnumsAndWellKnownTypesFromAssembly(SystemPrivateXml));
        list.AddRange(AddEnumsAndWellKnownTypesFromAssembly(SystemPrivateXmlLinq));

#if !(NET35 || NETSTANDARD1_3)
        var systemPrivateDataContractSerialization = typeof(Runtime.Serialization.DataContractResolver).GetTypeInfo().Assembly.FullName!;
        list.AddRange(AddEnumsAndWellKnownTypesFromAssembly(systemPrivateDataContractSerialization));
#endif
        foreach (var group in list.GroupBy(t => t.Name))
        {
            Add(group);
        }
    }

    private static List<Type> AddEnumsAndWellKnownTypesFromAssembly(string assemblyName)
    {
        try
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            var types = assembly.GetTypes().ToArray();

            var enumTypes = types.Where(t => t.GetTypeInfo().IsEnum && t.GetTypeInfo().IsPublic);
            var enumLikeTypes = FindEnumLikeTypes(types.Where(x => x == typeof(StringComparer)).ToArray());

            return enumTypes.Union(enumLikeTypes).ToList();
        }
        catch
        {
            return [];
        }
    }

    private static Type[] FindEnumLikeTypes(Type[] types)
    {
        try
        {
            return types
                .Where(t => t.GetTypeInfo().IsPublic && !t.GetTypeInfo().IsEnum && HasStaticInstancesOfOwnType(t))
                .ToArray();
        }
        catch
        {
            return [];
        }
    }

    private static bool HasStaticInstancesOfOwnType(Type type)
    {
        // Check for static properties that return the same type
        var anyStaticProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Any(p => p.PropertyType == type || p.PropertyType == type);

        if (anyStaticProperties)
        {
            return true;
        }

        // Check for static fields that return the same type
        var anyStaticFields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Any(f => f.FieldType == type || f.FieldType == type);

        return anyStaticFields;
    }

    private static void Add(IGrouping<string, Type> group)
    {
        if (group.Count() == 1)
        {
            var singleType = group.Single();
            PredefinedEnumerationTypes.TryAdd(group.Key, singleType);
            PredefinedEnumerationTypes.TryAdd(singleType.FullName!, singleType);
        }
        else
        {
            foreach (var fullType in group)
            {
                PredefinedEnumerationTypes.TryAdd(fullType.FullName!, fullType);
            }
        }
    }
}