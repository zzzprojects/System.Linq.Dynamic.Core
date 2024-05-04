using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.Parser;

internal static class PredefinedTypesHelper
{
    // These shorthands have different name than actual type and therefore not recognized by default from the PredefinedTypes.
    public static readonly IDictionary<string, Type> PredefinedTypesShorthands = new Dictionary<string, Type>
    {
        { "bool", typeof(bool) },
        { "byte", typeof(byte) },
        { "char", typeof(char) },
        { "decimal", typeof(decimal) },
        { "double", typeof(double) },
        { "float", typeof(float) },
        { "int", typeof(int) },
        { "long", typeof(long) },
        { "object", typeof(object) },
        { "sbyte", typeof(sbyte) },
        { "short", typeof(short) },
        { "string", typeof(string) },
        { "uint", typeof(uint) },
        { "ulong", typeof(ulong) },
        { "ushort", typeof(ushort) }
    };

    public static readonly IDictionary<Type, int> PredefinedTypes = new ConcurrentDictionary<Type, int>(new Dictionary<Type, int>
    {
        { typeof(object), 0 },
        { typeof(bool), 0 },
        { typeof(char), 0 },
        { typeof(string), 0 },
        { typeof(sbyte), 0 },
        { typeof(byte), 0 },
        { typeof(short), 0 },
        { typeof(ushort), 0 },
        { typeof(int), 0 },
        { typeof(uint), 0 },
        { typeof(long), 0 },
        { typeof(ulong), 0 },
        { typeof(float), 0 },
        { typeof(double), 0 },
        { typeof(decimal), 0 },
        { typeof(DateTime), 0 },
        { typeof(DateTimeOffset), 0 },
        { typeof(TimeSpan), 0 },
        { typeof(Guid), 0 },
        { typeof(Math), 0 },
        { typeof(Convert), 0 },
        { typeof(Uri), 0 },
        { typeof(Enum), 0 },
#if NET6_0_OR_GREATER
        { typeof(DateOnly), 0 },
        { typeof(TimeOnly), 0 }
#endif
    });

    static PredefinedTypesHelper()
    {
        // Only add these types for full .NET Framework and .NETStandard 2.1
        // And only if the EntityFramework.DynamicLinq is available.
#if NET452_OR_GREATER || NETSTANDARD2_1
        if (Type.GetType("EntityFramework.DynamicLinq.EFType, EntityFramework.DynamicLinq") != null)
        {
            TryAdd("System.Data.Objects.EntityFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            TryAdd("System.Data.Objects.SqlClient.SqlFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            TryAdd("System.Data.Objects.SqlClient.SqlSpatialFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            TryAdd("System.Data.Entity.Core.Objects.EntityFunctions, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            TryAdd("System.Data.Entity.DbFunctions, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            TryAdd("System.Data.Entity.Spatial.DbGeography, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            TryAdd("System.Data.Entity.SqlServer.SqlFunctions, EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            TryAdd("System.Data.Entity.SqlServer.SqlSpatialFunctions, EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
        }
#endif

#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        const string publicKeyToken = "974e7e1b462f3693";
        var version = Text.RegularExpressions.Regex.Match(typeof(PredefinedTypesHelper).AssemblyQualifiedName!, @"\d+\.\d+\.\d+\.\d+").ToString();
        if (Type.GetType($"Microsoft.EntityFrameworkCore.DynamicLinq.EFType, Microsoft.EntityFrameworkCore.DynamicLinq, Version={version}, Culture=neutral, PublicKeyToken={publicKeyToken}") != null)
        {
            TryAdd($"Microsoft.EntityFrameworkCore.DynamicLinq.DynamicFunctions, Microsoft.EntityFrameworkCore.DynamicLinq, Version={version}, Culture=neutral, PublicKeyToken={publicKeyToken}", 3);
        }
#endif
    }

    private static void TryAdd(string typeName, int x)
    {
        try
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                PredefinedTypes.Add(type, x);
            }
        }
        catch
        {
            // In case of exception, do not add
        }
    }

    public static bool IsPredefinedType(ParsingConfig config, Type type)
    {
        Check.NotNull(config);
        Check.NotNull(type);

        var nonNullableType = TypeHelper.GetNonNullableType(type);
        if (PredefinedTypes.ContainsKey(nonNullableType))
        {
            return true;
        }

        return config.CustomTypeProvider != null &&
               (config.CustomTypeProvider.GetCustomTypes().Contains(type) || config.CustomTypeProvider.GetCustomTypes().Contains(nonNullableType));
    }
}