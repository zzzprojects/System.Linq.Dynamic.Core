using System.IO;
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    internal class DefaultAssemblyHelper : IAssemblyHelper
    {
        public Assembly[] GetAssemblies()
        {
#if WINDOWS_APP || UAP10_0 || NETSTANDARD || WPSL
            throw new NotSupportedException();
#elif NET35

            return AppDomain.CurrentDomain.GetAssemblies();
#else
            // https://stackoverflow.com/a/2384679/255966
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var pathsToLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase));

            foreach (var path in pathsToLoad)
            {
                try
                {
                    loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
                }
                catch
                {
                    // Ignore
                }
            }

            return loadedAssemblies.ToArray();
#endif
        }
    }
}