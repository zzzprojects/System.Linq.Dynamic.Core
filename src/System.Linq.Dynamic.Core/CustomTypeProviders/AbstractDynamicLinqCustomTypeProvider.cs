using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders;

/// <summary>
/// The abstract DynamicLinqCustomTypeProvider which is used by the DefaultDynamicLinqCustomTypeProvider and can be used by a custom TypeProvider like in .NET Core.
/// </summary>
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public abstract class AbstractDynamicLinqCustomTypeProvider
{
    /// <summary>
    /// Additional types which should also be resolved.
    /// </summary>
    protected readonly IList<Type> AdditionalTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractDynamicLinqCustomTypeProvider"/> class.
    /// </summary>
    /// <param name="additionalTypes">A list of additional types (without the DynamicLinqTypeAttribute annotation) which should also be resolved.</param>
    protected AbstractDynamicLinqCustomTypeProvider(IList<Type> additionalTypes)
    {
        AdditionalTypes = Check.NotNull(additionalTypes);
    }

    /// <summary>
    /// Finds the unique types annotated with DynamicLinqTypeAttribute.
    /// </summary>
    /// <param name="assemblies">The assemblies to process.</param>
    /// <returns><see cref="IEnumerable{Type}" /></returns>
    protected Type[] FindTypesMarkedWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
    {
        Check.NotNull(assemblies);
#if !NET35
        assemblies = assemblies.Where(a => !a.IsDynamic);
#endif
        return GetAssemblyTypesWithDynamicLinqTypeAttribute(assemblies).Distinct().ToArray();
    }

    /// <summary>
    /// Resolve a type which is annotated with DynamicLinqTypeAttribute or when the type is listed in AdditionalTypes.
    /// </summary>
    /// <param name="assemblies">The assemblies to inspect.</param>
    /// <param name="typeName">The typename to resolve.</param>
    /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
    protected Type? ResolveType(IEnumerable<Assembly> assemblies, string typeName)
    {
        Check.NotNull(assemblies);
        Check.NotEmpty(typeName);

        var types = FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies).Union(AdditionalTypes);
        return types.FirstOrDefault(t => t.FullName == typeName);
    }

    /// <summary>
    /// Resolve a type which is annotated with DynamicLinqTypeAttribute by the simple (short) name.
    /// Also when the type is listed in AdditionalTypes.
    /// </summary>
    /// <param name="assemblies">The assemblies to inspect.</param>
    /// <param name="simpleTypeName">The simple typename to resolve.</param>
    /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
    protected Type? ResolveTypeBySimpleName(IEnumerable<Assembly> assemblies, string simpleTypeName)
    {
        Check.NotNull(assemblies);
        Check.NotEmpty(simpleTypeName);

        var types = FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies).Union(AdditionalTypes);
        var fullNames = types.Select(t => t.FullName!).Distinct().ToArray();
        var firstMatchingFullname = fullNames.FirstOrDefault(fn => fn.EndsWith($".{simpleTypeName}"));

        return firstMatchingFullname == null ? null : types.FirstOrDefault(t => t.FullName == firstMatchingFullname);
    }

#if (UAP10_0 || NETSTANDARD)
    /// <summary>
    /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
    /// </summary>
    /// <param name="assemblies">The assemblies to process.</param>
    /// <returns>Array of <see cref="Type" /></returns>
    protected Type[] GetAssemblyTypesWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
    {
        Check.NotNull(assemblies);

        var dynamicLinqTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            Type[] definedTypes;

            try
            {
                definedTypes = assembly.GetExportedTypes();
            }
            catch
            {
                // Ignore all other exceptions
                definedTypes = Type.EmptyTypes;
            }

            foreach (var definedType in definedTypes)
            {
                try
                {
                    if (definedType.GetTypeInfo().IsDefined(typeof(DynamicLinqTypeAttribute), false))
                    {
                        dynamicLinqTypes.Add(definedType);
                    }
                }
                catch
                {
                    // Ignore
                }
            }
        }

        return dynamicLinqTypes.Distinct().ToArray();
    }
#else                
    /// <summary>
    /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
    /// </summary>
    /// <param name="assemblies">The assemblies to process.</param>
    /// <returns>Array of <see cref="Type" /></returns>
    protected Type[] GetAssemblyTypesWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
    {
        Check.NotNull(assemblies);

        var dynamicLinqTypes = new List<Type>();

#if !NET5_0_OR_GREATER
        assemblies = assemblies.Where(a => !a.GlobalAssemblyCache).ToArray(); // Skip System DLL's
#endif
        foreach (var assembly in assemblies)
        {
            Type[] definedTypes;

            try
            {
                definedTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                definedTypes = reflectionTypeLoadException.Types.OfType<Type>().ToArray();
            }
            catch
            {
                // Ignore all other exceptions
                definedTypes = Type.EmptyTypes;
            }

            foreach (var definedType in definedTypes)
            {
                try
                {
                    if (definedType.IsDefined(typeof(DynamicLinqTypeAttribute), false))
                    {
                        dynamicLinqTypes.Add(definedType);
                    }
                }
                catch
                {
                    // Ignore
                }
            }
        }

        return dynamicLinqTypes.Distinct().ToArray();
    }
#endif
}