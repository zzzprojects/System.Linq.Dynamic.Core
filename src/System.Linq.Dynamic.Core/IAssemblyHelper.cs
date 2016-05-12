using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    public interface IAssemblyHelper
    {
        /// <summary>
        /// Gets the assemblies that have been loaded into the execution context of this application domain.
        /// </summary>
        /// 
        /// <returns>
        /// An array of assemblies in this application domain.
        /// </returns>
        Assembly[] GetAssemblies();
    }
}