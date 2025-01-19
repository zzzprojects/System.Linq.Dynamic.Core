using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal class TypeFinder : ITypeFinder
{
    private readonly IKeywordsHelper _keywordsHelper;
    private readonly ParsingConfig _parsingConfig;

    public TypeFinder(ParsingConfig parsingConfig, IKeywordsHelper keywordsHelper)
    {
        _parsingConfig = Check.NotNull(parsingConfig);
        _keywordsHelper = Check.NotNull(keywordsHelper);
    }

    public bool TryFindTypeByName(string name, ParameterExpression?[]? expressions, bool forceUseCustomTypeProvider, [NotNullWhen(true)] out Type? type)
    {
        Check.NotEmpty(name);

        if (_keywordsHelper.TryGetValue(name, out var keywordOrType) && keywordOrType.IsThird)
        {
            type = keywordOrType.Third;
            return true;
        }

        if (expressions != null && TryResolveTypeUsingExpressions(name, expressions, out type))
        {
            return true;
        }

        return TryResolveTypeByUsingCustomTypeProvider(name, forceUseCustomTypeProvider, out type);
    }

    private bool TryResolveTypeByUsingCustomTypeProvider(string name, bool forceUseCustomTypeProvider, [NotNullWhen(true)] out Type? resolvedType)
    {
        resolvedType = default;

        if ((forceUseCustomTypeProvider || _parsingConfig.AllowNewToEvaluateAnyType) && _parsingConfig.CustomTypeProvider != null)
        {
            resolvedType = _parsingConfig.CustomTypeProvider.ResolveType(name);
            if (resolvedType != null)
            {
                return true;
            }

            // In case the type is not found based on fullname, try to get the type on simple-name if allowed
            if (_parsingConfig.ResolveTypesBySimpleName)
            {
                resolvedType = _parsingConfig.CustomTypeProvider.ResolveTypeBySimpleName(name);
                return resolvedType != null;
            }
        }

        return false;
    }

    private bool TryResolveTypeUsingExpressions(string name, ParameterExpression?[] expressions, [NotNullWhen(true)] out Type? result)
    {
        foreach (var expression in expressions.OfType<Expression>())
        {
            if (name == expression.Type.Name)
            {
                result = expression.Type;
                return true;
            }

            if (name == $"{expression.Type.Namespace}.{expression.Type.Name}")
            {
                result = expression.Type;
                return true;
            }

            if (_parsingConfig is { ResolveTypesBySimpleName: true, CustomTypeProvider: not null })
            {
                var possibleFullName = $"{expression.Type.Namespace}.{name}";
                var resolvedType = _parsingConfig.CustomTypeProvider.ResolveType(possibleFullName);
                if (resolvedType != null)
                {
                    result = resolvedType;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}