using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace System.Linq.Dynamic.Core;

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
        var loadedPaths = new List<string>();
        foreach (var loadedAssembly in loadedAssemblies)
        {
            try
            {
                if (!loadedAssembly.IsDynamic)
                {
                    loadedPaths.Add(loadedAssembly.Location);
                }
            }
            catch
            {
                // Ignore
            }
        }

        string[] referencedPaths;
        try
        {
            referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        }
        catch
        {
            referencedPaths = new string[0];
        }

        var pathsToLoad = referencedPaths.Where(referencedPath => !loadedPaths.Contains(referencedPath, StringComparer.InvariantCultureIgnoreCase));
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