using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class KeywordsHelper : IKeywordsHelper
    {
        public const string SYMBOL_IT = "$";
        public const string SYMBOL_PARENT = "^";
        public const string SYMBOL_ROOT = "~";

        public const string KEYWORD_IT = "it";
        public const string KEYWORD_PARENT = "parent";
        public const string KEYWORD_ROOT = "root";

        public const string FUNCTION_IIF = "iif";
        public const string FUNCTION_ISNULL = "isnull";
        public const string FUNCTION_NEW = "new";
        public const string FUNCTION_NULLPROPAGATION = "np";
        public const string FUNCTION_IS = "is";
        public const string FUNCTION_AS = "as";
        public const string FUNCTION_CAST = "cast";

        private readonly IDictionary<string, object> _keywords = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "true", Expression.Constant(true) },
            { "false", Expression.Constant(false) },
            { "null", Expression.Constant(null) }
        };

        public KeywordsHelper(ParsingConfig config)
        {
            if (config.AreContextKeywordsEnabled)
            {
                _keywords.Add(KEYWORD_IT, KEYWORD_IT);
                _keywords.Add(KEYWORD_PARENT, KEYWORD_PARENT);
                _keywords.Add(KEYWORD_ROOT, KEYWORD_ROOT);
            }

            _keywords.Add(SYMBOL_IT, SYMBOL_IT);
            _keywords.Add(SYMBOL_PARENT, SYMBOL_PARENT);
            _keywords.Add(SYMBOL_ROOT, SYMBOL_ROOT);

            _keywords.Add(FUNCTION_IIF, FUNCTION_IIF);
            _keywords.Add(FUNCTION_ISNULL, FUNCTION_ISNULL);
            _keywords.Add(FUNCTION_NEW, FUNCTION_NEW);
            _keywords.Add(FUNCTION_NULLPROPAGATION, FUNCTION_NULLPROPAGATION);
            _keywords.Add(FUNCTION_IS, FUNCTION_IS);
            _keywords.Add(FUNCTION_AS, FUNCTION_AS);
            _keywords.Add(FUNCTION_CAST, FUNCTION_CAST);

            foreach (Type type in PredefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                if (!string.IsNullOrEmpty(type.FullName))
                {
                    _keywords[GetFormattedName(type, type.FullName)] = type;
                }
                _keywords[GetFormattedName(type, type.Name)] = type;
            }

            foreach (var pair in PredefinedTypesHelper.PredefinedTypesShorthands)
            {
                if (!_keywords.ContainsKey(pair.Key))
                {
                    _keywords.Add(pair.Key, pair.Value);
                }
            }

            if (config.SupportEnumerationsFromSystemNamespace)
            {
                foreach (var pair in EnumerationsFromMscorlib.PredefinedEnumerationTypes)
                {
                    _keywords.Add(pair.Key, pair.Value);
                }
            }

            if (config.CustomTypeProvider != null)
            {
                foreach (Type type in config.CustomTypeProvider.GetCustomTypes())
                {
                    _keywords[type.FullName] = type;
                    _keywords[type.Name] = type;
                }
            }
        }

        public bool TryGetValue(string name, out object type)
        {
            return _keywords.TryGetValue(name, out type);
        }

        public static string GetFormattedName(Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();
            
            if (typeInfo.IsGenericType)
            {
                var genericArguments = typeInfo
                    .GetGenericTypeArguments()
                    .Select(typeArgument => PredefinedTypesHelper.PredefinedTypesShorthands.FirstOrDefault(k => k.Value == typeArgument).Key ?? typeArgument.Name)
                    .ToArray();

                return $"{name.Substring(0, name.IndexOf("`"))}<{string.Join(", ", genericArguments)}>";
            }

            return name;
        }
    }
}
