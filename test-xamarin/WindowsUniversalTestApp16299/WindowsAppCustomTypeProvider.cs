using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;
using System.Threading.Tasks;

namespace WindowsUniversalTestApp16299
{
    class WindowsAppCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        public HashSet<Type> GetCustomTypes()
        {
            var assemblies = GetAssemblyListAsync().Result.Where(x => !x.IsDynamic).ToArray();

            return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
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
