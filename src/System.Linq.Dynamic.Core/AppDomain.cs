#if DNXCORE50 || NETFX
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.PlatformAbstractions;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

#if DNXCORE50
        public Assembly[] GetAssemblies()
        {
            var assemblyNames = PlatformServices.Default.LibraryManager.GetLibraries().SelectMany(lib => lib.Assemblies).Distinct().ToArray();

            var assemblies = new List<Assembly>();
            foreach (var assembly in assemblyNames.Select(Assembly.Load))
            {
                try
                {
                    var dummy = assembly.DefinedTypes.ToArray(); // just load all types and skip this assembly of one or more types cannot be resolved
                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                }
            }
            
            return assemblies.ToArray();
        }
#else
        public Assembly[] GetAssemblies()
        {
            return GetAssemblyListAsync().Result.ToArray();
        }

        private async System.Threading.Tasks.Task<IEnumerable<Assembly>> GetAssemblyListAsync()
        {
            var folder = Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    AssemblyName name = new AssemblyName() { Name = Path.GetFileNameWithoutExtension(file.Name) };
                    Assembly asm = Assembly.Load(name);
                    assemblies.Add(asm);
                }
            }

            return assemblies;
        }
#endif
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
        {
            return AssemblyBuilder.DefineDynamicAssembly(name, access);
        }
    }
}
#endif