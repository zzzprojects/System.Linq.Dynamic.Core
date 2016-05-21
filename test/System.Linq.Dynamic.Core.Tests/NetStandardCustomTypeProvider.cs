using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.InternalAbstractions;
using Microsoft.Extensions.DependencyModel;
using Xunit.Runner.DotNet;

namespace System.Linq.Dynamic.Core.Tests
{
    class NetStandardCustomTypeProvider : IDynamicLinkCustomTypeProvider
    {
        public HashSet<Type> GetCustomTypes()
        {
            var assemblies = GetAssemblyList().Where(x => !x.IsDynamic).ToArray();

            var definedTypes = assemblies.SelectMany(x => x.DefinedTypes);
            var types = definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());

            return new HashSet<Type>(types);
        }

        private static List<Assembly> GetAssemblyList()
        {
            List<Assembly> assemblies = new List<Assembly>();


            var dependencyModel = DependencyContext.Load(typeof(Program).GetTypeInfo().Assembly);

            foreach (var assemblyName in dependencyModel.GetRuntimeAssemblyNames(RuntimeEnvironment.GetRuntimeIdentifier()))
            {
                try
                {
                    try
                    {
                        var assembly = Assembly.Load(assemblyName);

                        // just load all types and skip this assembly of one or more types cannot be resolved
                        var dummy = assembly.DefinedTypes.ToArray();
                        assemblies.Add(assembly);
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                    }
                }
                catch
                {
                }
            }

            return assemblies;
        }
    }
}