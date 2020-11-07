using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class EnumerationsFromMscorlib
    {
        /// <summary>
        /// All Enum types from mscorlib/netstandard.
        /// </summary>
        public static readonly IDictionary<string, Type> PredefinedEnumerationTypes = new ConcurrentDictionary<string, Type>();
        

        static EnumerationsFromMscorlib()
        {
#if !(UAP10_0 || NETSTANDARD || NET35 || NETCOREAPP)
            var runtimeTypes = Assembly.Load("mscorlib").GetTypes().Where(t => t.IsEnum && t.IsPublic);
#else
#endif
        }

        public static readonly IDictionary<Type, int> PredefinedEnumerationTypes2 = new ConcurrentDictionary<Type, int>(new Dictionary<Type, int> {
#if !(UAP10_0 || NETSTANDARD || NET35 || NETCOREAPP)
            { typeof(AppDomainManagerInitializationOptions), 0 },
            { typeof(Base64FormattingOptions), 0 },
            { typeof(ConsoleColor), 0 },
            { typeof(ConsoleKey), 0 },
            { typeof(ConsoleModifiers), 0 },
            { typeof(ConsoleSpecialKey), 0 },
            { typeof(ActivationContext.ContextForm), 0 },
            { typeof(EnvironmentVariableTarget), 0 },
            { typeof(GCNotificationStatus), 0 },
            { typeof(LoaderOptimization), 0 },
            { typeof(PlatformID), 0 },
            { typeof(Environment.SpecialFolder), 0 },
            { typeof(Environment.SpecialFolderOption), 0 },
#endif
            { typeof(AttributeTargets), 0 },
            { typeof(DateTimeKind), 0 },
            { typeof(DayOfWeek), 0 },
            { typeof(GCCollectionMode), 0 },
            { typeof(MidpointRounding), 0 },
            { typeof(StringComparison), 0 },
            { typeof(StringSplitOptions), 0 },
            { typeof(TypeCode), 0 }
        });
    }
}
