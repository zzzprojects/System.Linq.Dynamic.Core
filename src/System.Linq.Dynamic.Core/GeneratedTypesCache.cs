using System.Collections.Concurrent;
using System.Threading;

namespace System.Linq.Dynamic.Core
{
    internal class GeneratedTypesCache
    {
        private static int _index = -1;

        private static readonly ConcurrentDictionary<string, Type> GeneratedTypes = new ConcurrentDictionary<string, Type>();

        internal bool TryGet(string[] propertyNames, out Type type)
        {
            type = null;
            // Anonymous classes are generics based. The generic classes are distinguished by number of parameters and name of parameters.
            // The specific types of the parameters are the generic arguments.
            // We recreate this by creating a fullName composed of all the property names, separated by a "|".
            string fullName = string.Join("|", propertyNames.Select(Escape).ToArray());

            lock (GeneratedTypes)
            {
                if (!GeneratedTypes.TryGetValue(fullName, out type))
                {
                    int index = Interlocked.Increment(ref _index);
                }
            }

            return true;
        }

        private static string Escape(string str)
        {
            // We escape the \ with \\, so that we can safely escape the "|" (that we use as a separator) with "\|"
            str = str.Replace(@"\", @"\\");
            str = str.Replace(@"|", @"\|");
            return str;
        }
    }
}