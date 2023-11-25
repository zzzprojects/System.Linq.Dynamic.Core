using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal class KeywordsHelper : IKeywordsHelper
{
    public const string KEYWORD_IT = "it";
    public const string KEYWORD_PARENT = "parent";
    public const string KEYWORD_ROOT = "root";

    public const string SYMBOL_IT = "$";
    public const string SYMBOL_PARENT = "^";
    public const string SYMBOL_ROOT = "~";

    public const string FUNCTION_IIF = "iif";
    public const string FUNCTION_ISNULL = "isnull";
    public const string FUNCTION_NEW = "new";
    public const string FUNCTION_NULLPROPAGATION = "np";
    public const string FUNCTION_IS = "is";
    public const string FUNCTION_AS = "as";
    public const string FUNCTION_CAST = "cast";

    private readonly ParsingConfig _config;

    // Keywords are IgnoreCase
    private readonly Dictionary<string, object> _keywordMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { "true", Expression.Constant(true) },
        { "false", Expression.Constant(false) },
        { "null", Expression.Constant(null) },

        { SYMBOL_IT, SYMBOL_IT },
        { SYMBOL_PARENT, SYMBOL_PARENT },
        { SYMBOL_ROOT, SYMBOL_ROOT },

        { FUNCTION_IIF, FUNCTION_IIF },
        { FUNCTION_ISNULL, FUNCTION_ISNULL },
        { FUNCTION_NEW, FUNCTION_NEW },
        { FUNCTION_NULLPROPAGATION, FUNCTION_NULLPROPAGATION },
        { FUNCTION_IS, FUNCTION_IS },
        { FUNCTION_AS, FUNCTION_AS },
        { FUNCTION_CAST, FUNCTION_CAST }
    };

    // PreDefined Types are not IgnoreCase
    private static readonly Dictionary<string, object> _preDefinedTypeMapping = new();

    // Custom DefinedTypes are not IgnoreCase
    private readonly Dictionary<string, object> _customTypeMapping = new();

    static KeywordsHelper()
    {
        foreach (var type in PredefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
        {
            _preDefinedTypeMapping[type.FullName!] = type;
            _preDefinedTypeMapping[type.Name] = type;
        }
    }

    public KeywordsHelper(ParsingConfig config)
    {
        _config = Check.NotNull(config);

        if (config.AreContextKeywordsEnabled)
        {
            _keywordMapping.Add(KEYWORD_IT, KEYWORD_IT);
            _keywordMapping.Add(KEYWORD_PARENT, KEYWORD_PARENT);
            _keywordMapping.Add(KEYWORD_ROOT, KEYWORD_ROOT);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (config.CustomTypeProvider != null)
        {
            foreach (var type in config.CustomTypeProvider.GetCustomTypes())
            {
                _customTypeMapping[type.FullName!] = type;
                _customTypeMapping[type.Name] = type;
            }
        }
    }

    public bool TryGetValue(string name, [NotNullWhen(true)] out object? keyWordOrType)
    {
        // 1. Try to get as keyword
        if (_keywordMapping.TryGetValue(name, out var keyWord))
        {
            keyWordOrType = keyWord;
            return true;
        }

        // 2. Try to get as predefined shorttype ("bool", "char", ...)
        if (PredefinedTypesHelper.PredefinedTypesShorthands.TryGetValue(name, out var predefinedShortHandType))
        {
            keyWordOrType = predefinedShortHandType;
            return true;
        }

        // 3. Try to get as predefined type ("Boolean", "System.Boolean", ..., "DateTime", "System.DateTime", ...)
        if (_preDefinedTypeMapping.TryGetValue(name, out var predefinedType))
        {
            keyWordOrType = predefinedType;
            return true;
        }

        // 4. Try to get as an enum from the system namespace
        if (_config.SupportEnumerationsFromSystemNamespace && EnumerationsFromMscorlib.PredefinedEnumerationTypes.TryGetValue(name, out var predefinedEnumType))
        {
            keyWordOrType = predefinedEnumType;
            return true;
        }

        // 5. Try to get as custom type
        if (_customTypeMapping.TryGetValue(name, out var customType))
        {
            keyWordOrType = customType;
            return true;
        }

        // 6. Not found, return false
        keyWordOrType = null;
        return false;
    }
}