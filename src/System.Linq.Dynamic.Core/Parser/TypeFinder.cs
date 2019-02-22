using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class TypeFinder : ITypeFinder
    {
        private readonly IKeywordsHelper _keywordsHelper;
        private readonly ParsingConfig _parsingConfig;

        public TypeFinder([NotNull] ParsingConfig parsingConfig, [NotNull] IKeywordsHelper keywordsHelper)
        {
            Check.NotNull(parsingConfig, nameof(parsingConfig));
            Check.NotNull(keywordsHelper, nameof(keywordsHelper));

            _keywordsHelper = keywordsHelper;
            _parsingConfig = parsingConfig;
        }

        public Type FindTypeByName(string name, ParameterExpression[] expressions, bool forceUseCustomTypeProvider = false)
        {
            Check.NotEmpty(name, nameof(name));

            _keywordsHelper.TryGetValue(name, out object type);

            Type result = type as Type;
            if (result != null)
            {
                return result;
            }

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    if (expression != null && expression.Type.Name == name)
                    {
                        return expression.Type;
                    }

                    if (expression != null && expression.Type.Namespace + "." + expression.Type.Name == name)
                    {
                        return expression.Type;
                    }
                }
            }

            if ((forceUseCustomTypeProvider || _parsingConfig.AllowNewToEvaluateAnyType) && _parsingConfig.CustomTypeProvider != null)
            {
                var resolvedType = _parsingConfig.CustomTypeProvider.ResolveType(name);
                if (resolvedType != null)
                {
                    return resolvedType;
                }

                if (_parsingConfig.ResolveTypesBySimpleName && resolvedType == null)
                {
                    return _parsingConfig.CustomTypeProvider.ResolveTypeBySimpleName(name);
                }
            }

            return null;
        }
    }
}
