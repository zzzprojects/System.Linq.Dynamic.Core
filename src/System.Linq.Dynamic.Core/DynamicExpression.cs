using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Helper class to convert an expression into an LambdaExpression
    /// </summary>
    public static class DynamicExpression
    {
        /// <summary>
        /// Parses a expression into a LambdaExpression.
        /// </summary>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="itType">The main type from the dynamic class expression.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(bool createParameterCtor, [NotNull] Type itType, [CanBeNull] Type resultType, [NotNull] string expression, params object[] values)
        {
            Check.NotNull(itType, nameof(itType));
            Check.NotEmpty(expression, nameof(expression));

            return ParseLambda(createParameterCtor, new[] { Expression.Parameter(itType, "") }, resultType, expression, values);
        }

        /// <summary>
        /// Parses a expression into a LambdaExpression.
        /// </summary>
        /// <param name="createParameterCtor">if set to <c>true</c> then also create a constructor for all the parameters. Note that this doesn't work for Linq-to-Database entities.</param>
        /// <param name="parameters">A array from ParameterExpressions.</param>
        /// <param name="resultType">Type of the result. If not specified, it will be generated dynamically.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="values">An object array that contains zero or more objects which are used as replacement values.</param>
        /// <returns>The generated <see cref="LambdaExpression"/></returns>
        public static LambdaExpression ParseLambda(bool createParameterCtor, [NotNull] ParameterExpression[] parameters, [CanBeNull] Type resultType, [NotNull] string expression, params object[] values)
        {
            Check.NotEmpty(parameters, nameof(parameters));
            Check.Condition(parameters, p => p.Count(x => x == null) == 0, nameof(parameters));
            Check.NotEmpty(expression, nameof(expression));

            var parser = new ExpressionParser(parameters, expression, values);

            return Expression.Lambda(parser.Parse(resultType, createParameterCtor), parameters);
        }
    }
}