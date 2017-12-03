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
        public const string KEYWORD_IIF = "iif";
        public const string KEYWORD_NEW = "new";
        public const string KEYWORD_ISNULL = "isnull";

        private readonly IDictionary<string, object> _keywords = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "true", Constants.TrueLiteral },
            { "false", Constants.FalseLiteral },
            { "null", Constants.NullLiteral }
        };

        public KeywordsHelper(ParsingConfig config, PredefinedTypesHelper predefinedTypesHelper)
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
            _keywords.Add(KEYWORD_IIF, KEYWORD_IIF);
            _keywords.Add(KEYWORD_NEW, KEYWORD_NEW);
            _keywords.Add(KEYWORD_ISNULL, KEYWORD_ISNULL);

            foreach (Type type in predefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                _keywords[type.FullName] = type;
                _keywords[type.Name] = type;
            }

            foreach (KeyValuePair<string, Type> pair in predefinedTypesHelper.PredefinedTypesShorthands)
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
