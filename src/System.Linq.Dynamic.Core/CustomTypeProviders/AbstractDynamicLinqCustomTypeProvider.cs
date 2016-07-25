using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.CustomTypeProviders
{
    /// <summary>
    /// The abstract <see cref="AbstractDynamicLinqCustomTypeProvider"/>. Find all types marked with <see cref="DynamicLinqTypeAttribute"/>.
    /// </summary>
    public abstract class AbstractDynamicLinqCustomTypeProvider
    {
        /// <summary>
        /// Finds the types marked with DynamicLinqTypeAttribute.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns>IEnumerable{Type}</returns>
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

#if (WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
        /// <summary>
        /// Gets the assembly types in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns>IEnumerable{Type}</returns>
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
                catch (Exception)
                { }

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
        /// <returns>IEnumerable{Type}</returns>
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
                catch (Exception)
                { }

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