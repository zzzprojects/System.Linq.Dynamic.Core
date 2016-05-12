using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UniversalWindowsApp
{
    class WindowsAppCustomTypeProvider: IDynamicLinkCustomTypeProvider
    {
        public HashSet<Type> GetCustomTypes()
        {
            var assemblies = GetAssemblyListAsync().Result.Where(x => !x.IsDynamic).ToArray();
            
            var definedTypes = assemblies.SelectMany(x => x.DefinedTypes);
            var types = definedTypes.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DynamicLinqTypeAttribute))).Select(x => x.AsType());

            return new HashSet<Type>(types);
        }

        private static async Task<List<Assembly>> GetAssemblyListAsync()
        {
            List<Assembly> assemblies = new List<Assembly>();

            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            if (files == null)
                return assemblies;

            foreach (var file in files.Where(file => file.FileType == ".dll" || file.FileType == ".exe"))
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(file.DisplayName));

                    // just load all types and skip this assembly of one or more types cannot be resolved
                    var dummy = assembly.DefinedTypes.ToArray();
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }
            }

            return assemblies;
        }
    }
}
