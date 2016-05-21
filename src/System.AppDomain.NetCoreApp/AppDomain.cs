using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.DotNet.InternalAbstractions;
using Microsoft.Extensions.DependencyModel;

namespace System.AppDomain.NetCoreApp
{
    public sealed class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        /// <summary>
        /// Gets the assemblies that have been loaded into the execution context of application domain from the supplied type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>An array of assemblies in the application domain from the supplied type.</returns>
        public Assembly[] GetAssemblies(Type type)
        {
            List<Assembly> assemblies = new List<Assembly>();

            var dependencyModel = DependencyContext.Load(type.GetTypeInfo().Assembly);

            foreach (var assemblyName in dependencyModel.GetRuntimeAssemblyNames(RuntimeEnvironment.GetRuntimeIdentifier()))
            {
                try
                {
                    try
                    {
                        var assembly = Assembly.Load(assemblyName);

                        // just load all types and skip this assembly if one or more types cannot be resolved
                        assembly.DefinedTypes.ToArray();
                        assemblies.Add(assembly);
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }
            }

            return assemblies.ToArray();
        }
    }
}