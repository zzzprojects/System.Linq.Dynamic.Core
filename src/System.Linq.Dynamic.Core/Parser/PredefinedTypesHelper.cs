using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class PredefinedTypesHelper
    {
        private readonly ParsingConfig _config;

        // These shorthands have different name than actual type and therefore not recognized by default from the _predefinedTypes
        public readonly IDictionary<string, Type> PredefinedTypesShorthands = new Dictionary<string, Type>
        {
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "short", typeof(short) },
            { "ushort", typeof(ushort) },
            { "long", typeof(long) },
            { "ulong", typeof(ulong) },
            { "bool", typeof(bool) },
            { "float", typeof(float) },
        };

        public readonly IDictionary<Type, int> PredefinedTypes = new ConcurrentDictionary<Type, int>(new Dictionary<Type, int> {
            { typeof(object), 0 },
            { typeof(bool), 0 },
            { typeof(char), 0 },
            { typeof(string), 0 },
            { typeof(sbyte), 0 },
            { typeof(byte), 0 },
            { typeof(short), 0 },
            { typeof(ushort), 0 },
            { typeof(int), 0 },
            { typeof(uint), 0 },
            { typeof(long), 0 },
            { typeof(ulong), 0 },
            { typeof(float), 0 },
            { typeof(double), 0 },
            { typeof(decimal), 0 },
            { typeof(DateTime), 0 },
            { typeof(DateTimeOffset), 0 },
            { typeof(TimeSpan), 0 },
            { typeof(Guid), 0 },
            { typeof(Math), 0 },
            { typeof(Convert), 0 },
            { typeof(Uri), 0 }
        });

        public PredefinedTypesHelper(ParsingConfig config)
        {
            _config = config;
        }

        public void TryAdd(string typeName, int x)
        {
            try
            {
                Type efType = Type.GetType(typeName);
                if (efType != null)
                {
                    PredefinedTypes.Add(efType, x);
                }
            }
            catch
            {
                // in case of exception, do not add
            }
        }

        public bool IsPredefinedType(Type type)
        {
            if (PredefinedTypes.ContainsKey(type))
            {
                return true;
            }

            if (_config.CustomTypeProvider != null && _config.CustomTypeProvider.GetCustomTypes().Contains(type))
            {
                return true;
            }

            return false;
        }
    }
}
