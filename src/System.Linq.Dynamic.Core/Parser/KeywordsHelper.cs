using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using AnyOfTypes;

namespace System.Linq.Dynamic.Core.Parser;

internal class KeywordsHelper : IKeywordsHelper
{
    public const string KEYWORD_IT = "it";
    public const string KEYWORD_PARENT = "parent";
    public const string KEYWORD_ROOT = "root";

    public const string SYMBOL_IT = "$";
    public const string SYMBOL_PARENT = "^";
    public const string SYMBOL_ROOT = "~";

    public const string FUNCTION_AS = "as";
    public const string FUNCTION_CAST = "cast";
    public const string FUNCTION_IIF = "iif";
    public const string FUNCTION_IS = "is";
    public const string FUNCTION_ISNULL = "isnull";
    public const string FUNCTION_NEW = "new";
    public const string FUNCTION_NULLPROPAGATION = "np";

    private readonly ParsingConfig _config;

    // Keywords, symbols and functions compare case depends on the value from ParsingConfig.IsCaseSensitive
    private readonly Dictionary<string, AnyOf<string, Expression, Type>> _mappings;

    // Pre-defined Types are not IgnoreCase
    private static readonly Dictionary<string, Type> PreDefinedTypeMapping = new();

    // Custom DefinedTypes are not IgnoreCase
    private readonly Dictionary<string, Type> _customTypeMapping = new();

    static KeywordsHelper()
    {
        foreach (var type in PredefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
        {
            PreDefinedTypeMapping[type.FullName!] = type;
            PreDefinedTypeMapping[type.Name] = type;
        }
    }

    public KeywordsHelper(ParsingConfig config)
    {
        _config = Check.NotNull(config);

        _mappings = new(config.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase)
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

        if (config.AreContextKeywordsEnabled)
        {
            _mappings.Add(KEYWORD_IT, KEYWORD_IT);
            _mappings.Add(KEYWORD_PARENT, KEYWORD_PARENT);
            _mappings.Add(KEYWORD_ROOT, KEYWORD_ROOT);
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

    public bool IsItOrRootOrParent(AnyOf<string, Expression, Type> value)
    {
        if (value.IsFirst)
        {
            return value.First is KEYWORD_IT or KEYWORD_ROOT or KEYWORD_PARENT or SYMBOL_IT or SYMBOL_PARENT or SYMBOL_ROOT;
        }

        return false;
    }

    public bool TryGetValue(string text, out AnyOf<string, Expression, Type> value)
    {
        // 1. Try to get as constant-expression, keyword, symbol or functions
        if (_mappings.TryGetValue(text, out var expressionOrKeywordOrSymbolOrFunction))
        {
            value = expressionOrKeywordOrSymbolOrFunction;
            return true;
        }

        // 2. Try to get as predefined short-type ("bool", "char", ...)
        if (PredefinedTypesHelper.PredefinedTypesShorthands.TryGetValue(text, out var predefinedShortHandType))
        {
            value = predefinedShortHandType;
            return true;
        }

        // 3. Try to get as predefined type ("Boolean", "System.Boolean", ..., "DateTime", "System.DateTime", ...)
        if (PreDefinedTypeMapping.TryGetValue(text, out var predefinedType))
        {
            value = predefinedType;
            return true;
        }

        // 4. Try to get as an enum from the system namespace
        if (_config.SupportEnumerationsFromSystemNamespace && EnumerationsFromMscorlib.PredefinedEnumerationTypes.TryGetValue(text, out var predefinedEnumType))
        {
            value = predefinedEnumType;
            return true;
        }

        // 5. Try to get as custom type
        if (_customTypeMapping.TryGetValue(text, out var customType))
        {
            value = customType;
            return true;
        }

        // Not found, return false
        value = default;
        return false;
    }
}