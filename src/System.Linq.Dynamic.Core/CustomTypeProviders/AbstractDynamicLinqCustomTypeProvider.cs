using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The abstract DynamicLinqCustomTypeProvider which is used by the DefaultDynamicLinqCustomTypeProvider and can be used by a custom TypeProvider like in .NET Core.
    /// </summary>
    public abstract class AbstractDynamicLinqCustomTypeProvider
    {
        /// <summary>
        /// Finds the unique types marked with DynamicLinqTypeAttribute.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> FindTypesMarkedWithDynamicLinqTypeAttribute([NotNull] IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));
#if !NET35
            assemblies = assemblies.Where(a => !a.IsDynamic);
#endif
            return GetAssemblyTypesWithDynamicLinqTypeAttribute(assemblies).Distinct().ToArray();
        }

        /// <summary>
        /// Resolve any type which is registered in the current application domain.
        /// </summary>
        /// <param name="assemblies">The assemblies to inspect.</param>
        /// <param name="typeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        protected Type ResolveType([NotNull] IEnumerable<Assembly> assemblies, [NotNull] string typeName)
        {
            Check.NotNull(assemblies, nameof(assemblies));
            Check.NotEmpty(typeName, nameof(typeName));

            foreach (var assembly in assemblies)
            {
                Type resolvedType = assembly.GetType(typeName, false, true);
                if (resolvedType != null)
                {
                    return resolvedType;
                }
            }

            return null;
        }

        /// <summary>
        /// Resolve a type by the simple name which is registered in the current application domain.
        /// </summary>
        /// <param name="assemblies">The assemblies to inspect.</param>
        /// <param name="simpleTypeName">The simple typename to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        protected Type ResolveTypeBySimpleName([NotNull] IEnumerable<Assembly> assemblies, [NotNull] string simpleTypeName)
        {
            Check.NotNull(assemblies, nameof(assemblies));
            Check.NotEmpty(simpleTypeName, nameof(simpleTypeName));

            foreach (var assembly in assemblies)
            {
                var fullnames = assembly.GetTypes().Select(t => t.FullName).Distinct();
                var firstMatchingFullname = fullnames.FirstOrDefault(fn => fn.EndsWith($".{simpleTypeName}"));

                if (firstMatchingFullname != null)
                {
                    Type resolvedType = assembly.GetType(firstMatchingFullname, false, true);
                    if (resolvedType != null)
                    {
                        return resolvedType;
                    }
                }
            }

            return null;
        }

#if (WINDOWS_APP || UAP10_0 || NETSTANDARD)
        /// <summary>
        /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> GetAssemblyTypesWithDynamicLinqTypeAttribute([NotNull] IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                Type[] definedTypes = null;

                try
                {
                    definedTypes = assembly.ExportedTypes.Where(t => t.GetTypeInfo().IsDefined(typeof(DynamicLinqTypeAttribute), false)).ToArray();
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    definedTypes = reflectionTypeLoadException.Types;
                }
                catch
                {
                    // Ignore all other exceptions
                }

                if (definedTypes != null && definedTypes.Length > 0)
                {
                    foreach (var definedType in definedTypes)
                    {
                        yield return definedType;
                    }
                }
            }
        }
#else                
        /// <summary>
        /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> GetAssemblyTypesWithDynamicLinqTypeAttribute([NotNull] IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies.Where(a => !a.GlobalAssemblyCache)) // Skip System DLL's
            {
                Type[] definedTypes = null;

                try
                {
                    definedTypes = assembly.GetExportedTypes()
                        .Where(t => t.IsDefined(typeof(DynamicLinqTypeAttribute), false)).ToArray();
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    definedTypes = reflectionTypeLoadException.Types;
                }
                catch
                {
                    // Ignore all other exceptions
                }

                if (definedTypes != null && definedTypes.Length > 0)
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
