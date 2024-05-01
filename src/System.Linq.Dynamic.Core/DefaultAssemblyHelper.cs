using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core;

internal class DefaultAssemblyHelper(ParsingConfig parsingConfig) : IAssemblyHelper
{
    private readonly ParsingConfig _config = Check.NotNull(parsingConfig);

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

        if (!_config.LoadAdditionalAssembliesFromCurrentDomainBaseDirectory)
        {
            return loadedAssemblies.ToArray();
        }

        string[] referencedPaths;
        try
        {
            referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        }
        catch
        {
            referencedPaths = [];
        }

        var pathsToLoad = referencedPaths
            .Where(referencedPath => !loadedPaths.Contains(referencedPath, StringComparer.InvariantCultureIgnoreCase))
            .ToArray();
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