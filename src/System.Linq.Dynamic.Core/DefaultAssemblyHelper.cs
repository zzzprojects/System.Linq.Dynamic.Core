using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    internal class DefaultAssemblyHelper : IAssemblyHelper
    {
#if DOTNET5_4
        public Assembly[] GetAssemblies()
        {
            var assemblyNames = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.LibraryManager.GetLibraries()
                .SelectMany(lib => lib.Assemblies)
                .Distinct()
                .ToArray();

            var assemblies = new System.Collections.Generic.List<Assembly>();
            foreach (var assembly in assemblyNames.Select(Assembly.Load))
            {
                try
                {
                    var dummy = assembly.DefinedTypes.ToArray();
                    // just load all types and skip this assembly of one or more types cannot be resolved
                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                }
            }

            return assemblies.ToArray();
        }

#elif (DOTNET5_1 || WINDOWS_APP || UAP10_0 || NETSTANDARD || WPSL)
        public Assembly[] GetAssemblies()
        {
            throw new NotSupportedException();
        }
#else
        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
#endif
    }
}