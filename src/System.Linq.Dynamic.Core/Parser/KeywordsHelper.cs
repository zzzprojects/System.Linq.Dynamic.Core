using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class KeywordsHelper
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
        public const string FUNCTION_OFTYPE = "OfType";

        private readonly IDictionary<string, object> _keywords = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "true", Constants.TrueLiteral },
            { "false", Constants.FalseLiteral },
            { "null", Constants.NullLiteral }
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

            foreach (Type type in PredefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                _keywords[type.FullName] = type;
                _keywords[type.Name] = type;
            }

            foreach (KeyValuePair<string, Type> pair in PredefinedTypesHelper.PredefinedTypesShorthands)
            {
                _keywords.Add(pair.Key, pair.Value);
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
    }
}
