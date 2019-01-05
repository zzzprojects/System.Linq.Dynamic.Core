using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The abstract DynamicLinqCustomTypeProvider which is used by the <see cref="DefaultDynamicLinqCustomTypeProvider"/> and can be used by a custom TypeProvider like in .NET Core.
    /// </summary>
    public abstract class AbstractDynamicLinqCustomTypeProvider
    {
        /// <summary>
        /// Finds the types marked with DynamicLinqTypeAttribute.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> FindTypesMarkedWithDynamicLinqTypeAttribute([NotNull] IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));
#if !NET35
            assemblies = assemblies.Where(x => !x.IsDynamic);
#endif
            var definedTypes = GetAssemblyTypes(assemblies);

#if (WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            return definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());
#else
            return definedTypes.Where(x => x.GetCustomAttributes(typeof(DynamicLinqTypeAttribute), false).Any());
#endif
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

            foreach (Assembly assembly in assemblies)
            {
                Type resolvedType = assembly.GetType(typeName, false, true);
                if (resolvedType != null)
                {
                    return resolvedType;
                }
            }

            return null;
        }

#if (WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
        /// <summary>
        /// Gets the assembly types in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<TypeInfo> GetAssemblyTypes([NotNull] IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                IEnumerable<TypeInfo> definedTypes = null;

                try
                {
                    definedTypes = assembly.DefinedTypes;
                }
                catch
                {
                    // Ignore error
                }

                if (definedTypes != null)
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
        /// Gets the assembly types in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> GetAssemblyTypes([NotNull] IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                IEnumerable<Type> definedTypes = null;

                try
                {
                    definedTypes = assembly.GetTypes();
                }
                catch
                {
                    // Ignore error
                }

                if (definedTypes != null)
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
