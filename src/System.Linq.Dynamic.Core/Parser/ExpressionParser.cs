using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser.SupportedMethods;
using System.Linq.Dynamic.Core.Parser.SupportedOperands;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class ExpressionParser
    {
        static readonly string methodOrderBy = nameof(Queryable.OrderBy);
        static readonly string methodOrderByDescending = nameof(Queryable.OrderByDescending);
        static readonly string methodThenBy = nameof(Queryable.ThenBy);
        static readonly string methodThenByDescending = nameof(Queryable.ThenByDescending);

        private readonly ParsingConfig _parsingConfig;
        private readonly KeywordsHelper _keywordsHelper;
        private readonly PredefinedTypesHelper _predefinedTypesHelper;
        private readonly TextParser _textParser;
        private readonly Dictionary<string, object> _internals;
        private readonly Dictionary<string, object> _symbols;

        private IDictionary<string, object> _externals;
        private ParameterExpression _it;
        private ParameterExpression _parent;
        private ParameterExpression _root;
        private Type _resultType;
        private bool _createParameterCtor;

        public ExpressionParser([CanBeNull] ParameterExpression[] parameters, [NotNull] string expression, [CanBeNull] object[] values, [CanBeNull] ParsingConfig parsingConfig)
        {
            Check.NotEmpty(expression, nameof(expression));

            _symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _internals = new Dictionary<string, object>();

            if (parameters != null)
            {
                ProcessParameters(parameters);
            }

            if (values != null)
            {
                ProcessValues(values);
            }

            _parsingConfig = parsingConfig ?? ParsingConfig.Default;

            _predefinedTypesHelper = new PredefinedTypesHelper(_parsingConfig);
            InitPredefinedTypes();

            _keywordsHelper = new KeywordsHelper(_parsingConfig, _predefinedTypesHelper);
            _textParser = new TextParser(expression);
        }

        void InitPredefinedTypes()
        {
#if !(NET35 || SILVERLIGHT || NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            //System.Data.Entity is always here, so overwrite short name of it with EntityFramework if EntityFramework is found.
            //EF5(or 4.x??), System.Data.Objects.DataClasses.EdmFunctionAttribute
            //There is also an System.Data.Entity, Version=3.5.0.0, but no Functions.
            _predefinedTypesHelper.TryAdd("System.Data.Objects.EntityFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            _predefinedTypesHelper.TryAdd("System.Data.Objects.SqlClient.SqlFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            _predefinedTypesHelper.TryAdd("System.Data.Objects.SqlClient.SqlSpatialFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);

            //EF6,System.Data.Entity.DbFunctionAttribute
            _predefinedTypesHelper.TryAdd("System.Data.Entity.Core.Objects.EntityFunctions, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            _predefinedTypesHelper.TryAdd("System.Data.Entity.DbFunctions, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            _predefinedTypesHelper.TryAdd("System.Data.Entity.Spatial.DbGeography, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            _predefinedTypesHelper.TryAdd("System.Data.Entity.SqlServer.SqlFunctions, EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            _predefinedTypesHelper.TryAdd("System.Data.Entity.SqlServer.SqlSpatialFunctions, EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
#endif

#if NETSTANDARD2_0
            _predefinedTypesHelper.TryAdd("Microsoft.EntityFrameworkCore.DynamicLinq.DynamicFunctions, Microsoft.EntityFrameworkCore.DynamicLinq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=974e7e1b462f3693", 3);
#endif
            // detect NumberDecimalSeparator from current culture
            //NumberFormatInfo numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
            //NumberDecimalSeparatorFromCulture = numberFormatInfo.NumberDecimalSeparator[0];
        }

        void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression pe in parameters.Where(p => !string.IsNullOrEmpty(p.Name)))
            {
                AddSymbol(pe.Name, pe);
            }

            // If there is only 1 ParameterExpression, do also allow access using 'it'
            if (parameters.Length == 1)
            {
                _parent = _it;
                _it = parameters[0];

                if (_root == null)
                {
                    _root = _it;
                }
            }
        }

        void ProcessValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object value = values[i];
                IDictionary<string, object> externals;

                if (i == values.Length - 1 && (externals = value as IDictionary<string, object>) != null)
                {
                    _externals = externals;
                }
                else
                {
                    AddSymbol("@" + i.ToString(CultureInfo.InvariantCulture), value);
                }
            }
        }

        void AddSymbol(string name, object value)
        {
            if (_symbols.ContainsKey(name))
            {
                throw ParseError(Res.DuplicateIdentifier, name);
            }

            _symbols.Add(name, value);
        }

        public Expression Parse(Type resultType, bool createParameterCtor)
        {
            _resultType = resultType;
            _createParameterCtor = createParameterCtor;

            int exprPos = _textParser.CurrentToken.Pos;
            Expression expr = ParseConditionalOperator();

            if (resultType != null)
            {
                if ((expr = ExpressionPromoter.Promote(expr, resultType, true, false)) == null)
                {
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch, TypeHelper.GetTypeName(resultType));
                }
            }

            _textParser.ValidateToken(TokenId.End, Res.SyntaxError);

            return expr;
        }

#pragma warning disable 0219
        public IList<DynamicOrdering> ParseOrdering(bool forceThenBy = false)
        {
            var orderings = new List<DynamicOrdering>();
            while (true)
            {
                Expression expr = ParseConditionalOperator();
                bool ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    _textParser.NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    _textParser.NextToken();
                    ascending = false;
                }

                string methodName;
                if (forceThenBy || orderings.Count > 0)
                {
                    methodName = ascending ? methodThenBy : methodThenByDescending;
                }
                else
                {
                    methodName = ascending ? methodOrderBy : methodOrderByDescending;
                }

                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending, MethodName = methodName });

                if (_textParser.CurrentToken.Id != TokenId.Comma)
                {
                    break;
                }

                _textParser.NextToken();
            }

            _textParser.ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }
#pragma warning restore 0219

        // ?: operator
        Expression ParseConditionalOperator()
        {
            int errorPos = _textParser.CurrentToken.Pos;
            Expression expr = ParseNullCoalescingOperator();
            if (_textParser.CurrentToken.Id == TokenId.Question)
            {
                _textParser.NextToken();
                Expression expr1 = ParseConditionalOperator();
                _textParser.ValidateToken(TokenId.Colon, Res.ColonExpected);
                _textParser.NextToken();
                Expression expr2 = ParseConditionalOperator();
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        // ?? (null-coalescing) operator
        Expression ParseNullCoalescingOperator()
        {
            Expression expr = ParseLambdaOperator();
            if (_textParser.CurrentToken.Id == TokenId.NullCoalescing)
            {
                _textParser.NextToken();
                Expression right = ParseConditionalOperator();
                expr = Expression.Coalesce(expr, right);
            }
            return expr;
        }

        // => operator - Added Support for projection operator
        Expression ParseLambdaOperator()
        {
            Expression expr = ParseOrOperator();
            if (_textParser.CurrentToken.Id == TokenId.Lambda && _it.Type == expr.Type)
            {
                _textParser.NextToken();
                if (_textParser.CurrentToken.Id == TokenId.Identifier || _textParser.CurrentToken.Id == TokenId.OpenParen)
                {
                    var right = ParseConditionalOperator();
                    return Expression.Lambda(right, new[] { (ParameterExpression)expr });
                }
                _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            }
            return expr;
        }

        // isnull(a,b) operator
        Expression ParseIsNull()
        {
            int errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 2)
            {
                throw ParseError(errorPos, Res.IsNullRequiresTwoArgs);
            }

            return Expression.Coalesce(args[0], args[1]);
        }

        // ||, or operator
        Expression ParseOrOperator()
        {
            Expression left = ParseAndOperator();
            while (_textParser.CurrentToken.Id == TokenId.DoubleBar || TokenIdentifierIs("or"))
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseAndOperator();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        // &&, and operator
        Expression ParseAndOperator()
        {
            Expression left = ParseIn();
            while (_textParser.CurrentToken.Id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseComparisonOperator();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        // in operator for literals - example: "x in (1,2,3,4)"
        // in operator to mimic contains - example: "x in @0", compare to @0.Contains(x)
        // Adapted from ticket submitted by github user mlewis9548 
        Expression ParseIn()
        {
            Expression left = ParseLogicalAndOrOperator();
            Expression accumulate = left;

            while (TokenIdentifierIs("in"))
            {
                var op = _textParser.CurrentToken;

                _textParser.NextToken();
                if (_textParser.CurrentToken.Id == TokenId.OpenParen) // literals (or other inline list)
                {
                    while (_textParser.CurrentToken.Id != TokenId.CloseParen)
                    {
                        _textParser.NextToken();

                        // we need to parse unary expressions because otherwise 'in' clause will fail in use cases like 'in (-1, -1)' or 'in (!true)'
                        Expression right = ParseUnary();

                        // if the identifier is an Enum, try to convert the right-side also to an Enum.
                        if (left.Type.GetTypeInfo().IsEnum && right is ConstantExpression)
                        {
                            right = ParseEnumToConstantExpression(op.Pos, left.Type, right as ConstantExpression);
                        }

                        // else, check for direct type match
                        else if (left.Type != right.Type)
                        {
                            CheckAndPromoteOperands(typeof(IEqualitySignatures), "==", ref left, ref right, op.Pos);
                        }

                        if (accumulate.Type != typeof(bool))
                        {
                            accumulate = ExpressionHelper.GenerateEqual(left, right);
                        }
                        else
                        {
                            accumulate = Expression.OrElse(accumulate, ExpressionHelper.GenerateEqual(left, right));
                        }

                        if (_textParser.CurrentToken.Id == TokenId.End)
                        {
                            throw ParseError(op.Pos, Res.CloseParenOrCommaExpected);
                        }
                    }
                }
                else if (_textParser.CurrentToken.Id == TokenId.Identifier) // a single argument
                {
                    Expression right = ParsePrimary();

                    if (!typeof(IEnumerable).IsAssignableFrom(right.Type))
                    {
                        throw ParseError(_textParser.CurrentToken.Pos, Res.IdentifierImplementingInterfaceExpected, typeof(IEnumerable));
                    }

                    var args = new[] { left };

                    if (MethodFinder.FindMethod(typeof(IEnumerableSignatures), "Contains", false, args, out MethodBase containsSignature) != 1)
                    {
                        throw ParseError(op.Pos, Res.NoApplicableAggregate, "Contains");
                    }

                    var typeArgs = new[] { left.Type };

                    args = new[] { right, left };

                    accumulate = Expression.Call(typeof(Enumerable), containsSignature.Name, typeArgs, args);
                }
                else
                {
                    throw ParseError(op.Pos, Res.OpenParenOrIdentifierExpected);
                }

                _textParser.NextToken();
            }

            return accumulate;
        }

        // &, | bitwise operators
        Expression ParseLogicalAndOrOperator()
        {
            Expression left = ParseComparisonOperator();
            while (_textParser.CurrentToken.Id == TokenId.Amphersand || _textParser.CurrentToken.Id == TokenId.Bar)
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseComparisonOperator();

                if (left.Type.GetTypeInfo().IsEnum)
                {
                    left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));
                }

                if (right.Type.GetTypeInfo().IsEnum)
                {
                    right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));
                }

                switch (op.Id)
                {
                    case TokenId.Amphersand:
                        // When at least one side of the operator is a string, consider it's a VB-style concatenation operator.
                        // Doesn't break any other function since logical AND with a string is invalid anyway.
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            left = ExpressionHelper.GenerateStringConcat(left, right);
                        }
                        else
                        {
                            ExpressionHelper.ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref left, ref right);
                            left = Expression.And(left, right);
                        }
                        break;

                    case TokenId.Bar:
                        ExpressionHelper.ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref left, ref right);
                        left = Expression.Or(left, right);
                        break;
                }
            }
            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        Expression ParseComparisonOperator()
        {
            Expression left = ParseShiftOperator();
            while (_textParser.CurrentToken.Id == TokenId.Equal || _textParser.CurrentToken.Id == TokenId.DoubleEqual ||
                   _textParser.CurrentToken.Id == TokenId.ExclamationEqual || _textParser.CurrentToken.Id == TokenId.LessGreater ||
                   _textParser.CurrentToken.Id == TokenId.GreaterThan || _textParser.CurrentToken.Id == TokenId.GreaterThanEqual ||
                   _textParser.CurrentToken.Id == TokenId.LessThan || _textParser.CurrentToken.Id == TokenId.LessThanEqual)
            {
                ConstantExpression constantExpr;
                TypeConverter typeConverter;
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseShiftOperator();
                bool isEquality = op.Id == TokenId.Equal || op.Id == TokenId.DoubleEqual || op.Id == TokenId.ExclamationEqual;

                if (isEquality && (!left.Type.GetTypeInfo().IsValueType && !right.Type.GetTypeInfo().IsValueType || left.Type == typeof(Guid) && right.Type == typeof(Guid)))
                {
                    // If left or right is NullLiteral, just continue. Else check if the types differ.
                    if (!(left == Constants.NullLiteral || right == Constants.NullLiteral) && left.Type != right.Type)
                    {
                        if (left.Type.IsAssignableFrom(right.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (right.Type.IsAssignableFrom(left.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else if (TypeHelper.IsEnumType(left.Type) || TypeHelper.IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression e;
                        if ((e = ExpressionPromoter.Promote(right, left.Type, true, false)) != null)
                        {
                            right = e;
                        }
                        else if ((e = ExpressionPromoter.Promote(left, right.Type, true, false)) != null)
                        {
                            left = e;
                        }
                        else if (TypeHelper.IsEnumType(left.Type) && (constantExpr = right as ConstantExpression) != null)
                        {
                            right = ParseEnumToConstantExpression(op.Pos, left.Type, constantExpr);
                        }
                        else if (TypeHelper.IsEnumType(right.Type) && (constantExpr = left as ConstantExpression) != null)
                        {
                            left = ParseEnumToConstantExpression(op.Pos, right.Type, constantExpr);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else if ((constantExpr = right as ConstantExpression) != null && constantExpr.Value is string && (typeConverter = TypeConverterFactory.GetConverter(left.Type)) != null)
                {
                    right = Expression.Constant(typeConverter.ConvertFromInvariantString((string)constantExpr.Value), left.Type);
                }
                else if ((constantExpr = left as ConstantExpression) != null && constantExpr.Value is string && (typeConverter = TypeConverterFactory.GetConverter(right.Type)) != null)
                {
                    left = Expression.Constant(typeConverter.ConvertFromInvariantString((string)constantExpr.Value), right.Type);
                }
                else
                {
                    bool typesAreSameAndImplementCorrectInterface = false;
                    if (left.Type == right.Type)
                    {
                        var interfaces = left.Type.GetInterfaces().Where(x => x.GetTypeInfo().IsGenericType);
                        if (isEquality)
                        {
                            typesAreSameAndImplementCorrectInterface = interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IEquatable<>));
                        }
                        else
                        {
                            typesAreSameAndImplementCorrectInterface = interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(IComparable<>));
                        }
                    }

                    if (!typesAreSameAndImplementCorrectInterface)
                    {
                        CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), op.Text, ref left, ref right, op.Pos);
                    }
                }

                switch (op.Id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = ExpressionHelper.GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = ExpressionHelper.GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = ExpressionHelper.GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = ExpressionHelper.GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = ExpressionHelper.GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = ExpressionHelper.GenerateLessThanEqual(left, right);
                        break;
                }
            }

            return left;
        }

        private ConstantExpression ParseEnumToConstantExpression(int pos, Type leftType, ConstantExpression constantExpr)
        {
            return Expression.Constant(ParseConstantExpressionToEnum(pos, leftType, constantExpr), leftType);
        }

        private object ParseConstantExpressionToEnum(int pos, Type leftType, ConstantExpression constantExpr)
        {
            try
            {
                if (constantExpr.Value is string)
                {
                    return Enum.Parse(TypeHelper.GetNonNullableType(leftType), (string)constantExpr.Value, true);
                }

                return Enum.ToObject(leftType, constantExpr.Value);
            }
            catch
            {
                throw ParseError(pos, Res.ExpressionTypeMismatch, leftType);
            }
        }

        // <<, >> operators
        Expression ParseShiftOperator()
        {
            Expression left = ParseAdditive();
            while (_textParser.CurrentToken.Id == TokenId.DoubleLessThan || _textParser.CurrentToken.Id == TokenId.DoubleGreaterThan)
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseAdditive();
                switch (op.Id)
                {
                    case TokenId.DoubleLessThan:
                        CheckAndPromoteOperands(typeof(IShiftSignatures), op.Text, ref left, ref right, op.Pos);
                        left = Expression.LeftShift(left, right);
                        break;
                    case TokenId.DoubleGreaterThan:
                        CheckAndPromoteOperands(typeof(IShiftSignatures), op.Text, ref left, ref right, op.Pos);
                        left = Expression.RightShift(left, right);
                        break;
                }
            }
            return left;
        }

        // +, - operators
        Expression ParseAdditive()
        {
            Expression left = ParseMultiplicative();
            while (_textParser.CurrentToken.Id == TokenId.Plus || _textParser.CurrentToken.Id == TokenId.Minus)
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseMultiplicative();
                switch (op.Id)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            left = ExpressionHelper.GenerateStringConcat(left, right);
                        }
                        else
                        {
                            CheckAndPromoteOperands(typeof(IAddSignatures), op.Text, ref left, ref right, op.Pos);
                            left = ExpressionHelper.GenerateAdd(left, right);
                        }
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.Text, ref left, ref right, op.Pos);
                        left = ExpressionHelper.GenerateSubtract(left, right);
                        break;
                }
            }
            return left;
        }

        // *, /, %, mod operators
        Expression ParseMultiplicative()
        {
            Expression left = ParseUnary();
            while (_textParser.CurrentToken.Id == TokenId.Asterisk || _textParser.CurrentToken.Id == TokenId.Slash ||
                _textParser.CurrentToken.Id == TokenId.Percent || TokenIdentifierIs("mod"))
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                Expression right = ParseUnary();
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.Text, ref left, ref right, op.Pos);
                switch (op.Id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Percent:
                    case TokenId.Identifier:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        // -, !, not unary operators
        Expression ParseUnary()
        {
            if (_textParser.CurrentToken.Id == TokenId.Minus || _textParser.CurrentToken.Id == TokenId.Exclamation || TokenIdentifierIs("not"))
            {
                Token op = _textParser.CurrentToken;
                _textParser.NextToken();
                if (op.Id == TokenId.Minus && (_textParser.CurrentToken.Id == TokenId.IntegerLiteral || _textParser.CurrentToken.Id == TokenId.RealLiteral))
                {
                    _textParser.CurrentToken.Text = "-" + _textParser.CurrentToken.Text;
                    _textParser.CurrentToken.Pos = op.Pos;
                    return ParsePrimary();
                }

                Expression expr = ParseUnary();
                if (op.Id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Not(expr);
                }

                return expr;
            }

            return ParsePrimary();
        }

        Expression ParsePrimary()
        {
            Expression expr = ParsePrimaryStart();
            while (true)
            {
                if (_textParser.CurrentToken.Id == TokenId.Dot)
                {
                    _textParser.NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (_textParser.CurrentToken.Id == TokenId.NullPropagation)
                {
                    throw new NotSupportedException("An expression tree lambda may not contain a null propagating operator");
                }
                else if (_textParser.CurrentToken.Id == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        Expression ParsePrimaryStart()
        {
            switch (_textParser.CurrentToken.Id)
            {
                case TokenId.Identifier:
                    return ParseIdentifier();
                case TokenId.StringLiteral:
                    return ParseStringLiteral();
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral();
                case TokenId.RealLiteral:
                    return ParseRealLiteral();
                case TokenId.OpenParen:
                    return ParseParenExpression();
                default:
                    throw ParseError(Res.ExpressionExpected);
            }
        }

        Expression ParseStringLiteral()
        {
            _textParser.ValidateToken(TokenId.StringLiteral);
            char quote = _textParser.CurrentToken.Text[0];
            string s = _textParser.CurrentToken.Text.Substring(1, _textParser.CurrentToken.Text.Length - 2);

            if (quote == '\'')
            {
                if (s.Length != 1)
                {
                    throw ParseError(Res.InvalidCharacterLiteral);
                }
                _textParser.NextToken();
                return ConstantExpressionHelper.CreateLiteral(s[0], s);
            }
            _textParser.NextToken();
            return ConstantExpressionHelper.CreateLiteral(s, s);
        }

        Expression ParseIntegerLiteral()
        {
            _textParser.ValidateToken(TokenId.IntegerLiteral);

            string text = _textParser.CurrentToken.Text;
            string qualifier = null;
            char last = text[text.Length - 1];
            bool isHexadecimal = text.StartsWith(text[0] == '-' ? "-0x" : "0x", StringComparison.OrdinalIgnoreCase);
            char[] qualifierLetters = isHexadecimal
                                          ? new[] { 'U', 'u', 'L', 'l' }
                                          : new[] { 'U', 'u', 'L', 'l', 'F', 'f', 'D', 'd', 'M', 'm' };

            if (qualifierLetters.Contains(last))
            {
                int pos = text.Length - 1, count = 0;
                while (qualifierLetters.Contains(text[pos]))
                {
                    ++count;
                    --pos;
                }
                qualifier = text.Substring(text.Length - count, count);
                text = text.Substring(0, text.Length - count);
            }

            if (text[0] != '-')
            {
                if (isHexadecimal)
                {
                    text = text.Substring(2);
                }

                if (!ulong.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, CultureInfo.CurrentCulture, out ulong value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                }

                _textParser.NextToken();
                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "U" || qualifier == "u") return ConstantExpressionHelper.CreateLiteral((uint)value, text);
                    if (qualifier == "L" || qualifier == "l") return ConstantExpressionHelper.CreateLiteral((long)value, text);

                    // in case of UL, just return
                    return ConstantExpressionHelper.CreateLiteral(value, text);
                }

                // if (value <= (int)short.MaxValue) return ConstantExpressionHelper.CreateLiteral((short)value, text);
                if (value <= int.MaxValue) return ConstantExpressionHelper.CreateLiteral((int)value, text);
                if (value <= uint.MaxValue) return ConstantExpressionHelper.CreateLiteral((uint)value, text);
                if (value <= long.MaxValue) return ConstantExpressionHelper.CreateLiteral((long)value, text);

                return ConstantExpressionHelper.CreateLiteral(value, text);
            }
            else
            {
                if (isHexadecimal)
                {
                    text = text.Substring(3);
                }

                if (!long.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, CultureInfo.CurrentCulture, out long value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                }

                if (isHexadecimal)
                {
                    value = -value;
                }

                _textParser.NextToken();
                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "L" || qualifier == "l")
                        return ConstantExpressionHelper.CreateLiteral(value, text);

                    if (qualifier == "F" || qualifier == "f")
                        return TryParseAsFloat(text, qualifier[0]);

                    if (qualifier == "D" || qualifier == "d")
                        return TryParseAsDouble(text, qualifier[0]);

                    if (qualifier == "M" || qualifier == "m")
                        return TryParseAsDecimal(text, qualifier[0]);

                    throw ParseError(Res.MinusCannotBeAppliedToUnsignedInteger);
                }

                if (value <= int.MaxValue)
                {
                    return ConstantExpressionHelper.CreateLiteral((int)value, text);
                }

                return ConstantExpressionHelper.CreateLiteral(value, text);
            }
        }

        Expression ParseRealLiteral()
        {
            _textParser.ValidateToken(TokenId.RealLiteral);

            string text = _textParser.CurrentToken.Text;
            char qualifier = text[text.Length - 1];

            _textParser.NextToken();
            return TryParseAsFloat(text, qualifier);
        }

        Expression TryParseAsFloat(string text, char qualifier)
        {
            if (qualifier == 'F' || qualifier == 'f')
            {
                if (float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                {
                    return ConstantExpressionHelper.CreateLiteral(f, text);
                }
            }

            // not possible to find float qualifier, so try to parse as double
            return TryParseAsDecimal(text, qualifier);
        }

        Expression TryParseAsDecimal(string text, char qualifier)
        {
            if (qualifier == 'M' || qualifier == 'm')
            {
                if (decimal.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal d))
                {
                    return ConstantExpressionHelper.CreateLiteral(d, text);
                }
            }

            // not possible to find float qualifier, so try to parse as double
            return TryParseAsDouble(text, qualifier);
        }

        Expression TryParseAsDouble(string text, char qualifier)
        {
            double d;
            if (qualifier == 'D' || qualifier == 'd')
            {
                if (double.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                {
                    return ConstantExpressionHelper.CreateLiteral(d, text);
                }
            }

            if (double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
            {
                return ConstantExpressionHelper.CreateLiteral(d, text);
            }

            throw ParseError(Res.InvalidRealLiteral, text);
        }

        Expression ParseParenExpression()
        {
            _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            _textParser.NextToken();
            Expression e = ParseConditionalOperator();
            _textParser.ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            _textParser.NextToken();
            return e;
        }

        Expression ParseIdentifier()
        {
            _textParser.ValidateToken(TokenId.Identifier);

            if (_keywordsHelper.TryGetValue(_textParser.CurrentToken.Text, out object value))
            {
                var typeValue = value as Type;
                if (typeValue != null)
                {
                    return ParseTypeAccess(typeValue);
                }

                if (value == (object)KeywordsHelper.KEYWORD_IT) return ParseIt();
                if (value == (object)KeywordsHelper.KEYWORD_PARENT) return ParseParent();
                if (value == (object)KeywordsHelper.KEYWORD_ROOT) return ParseRoot();
                if (value == (object)KeywordsHelper.SYMBOL_IT) return ParseIt();
                if (value == (object)KeywordsHelper.SYMBOL_PARENT) return ParseParent();
                if (value == (object)KeywordsHelper.SYMBOL_ROOT) return ParseRoot();
                if (value == (object)KeywordsHelper.KEYWORD_IIF) return ParseIif();
                if (value == (object)KeywordsHelper.KEYWORD_NEW) return ParseNew();
                if (value == (object)KeywordsHelper.KEYWORD_ISNULL) return ParseIsNull();

                _textParser.NextToken();

                return (Expression)value;
            }

            if (_symbols.TryGetValue(_textParser.CurrentToken.Text, out value) ||
                _externals != null && _externals.TryGetValue(_textParser.CurrentToken.Text, out value) ||
                _internals.TryGetValue(_textParser.CurrentToken.Text, out value))
            {
                Expression expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    LambdaExpression lambda = expr as LambdaExpression;
                    if (lambda != null)
                    {
                        return ParseLambdaInvocation(lambda);
                    }
                }

                _textParser.NextToken();

                return expr;
            }

            if (_it != null)
            {
                return ParseMemberAccess(null, _it);
            }

            throw ParseError(Res.UnknownIdentifier, _textParser.CurrentToken.Text);
        }

        Expression ParseIt()
        {
            if (_it == null)
            {
                throw ParseError(Res.NoItInScope);
            }
            _textParser.NextToken();
            return _it;
        }

        Expression ParseParent()
        {
            if (_parent == null)
            {
                throw ParseError(Res.NoParentInScope);
            }
            _textParser.NextToken();
            return _parent;
        }

        Expression ParseRoot()
        {
            if (_root == null)
            {
                throw ParseError(Res.NoRootInScope);
            }
            _textParser.NextToken();
            return _root;
        }

        Expression ParseIif()
        {
            int errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 3)
            {
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            }

            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
            {
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            }

            if (expr1.Type != expr2.Type)
            {
                Expression expr1As2 = expr2 != Constants.NullLiteral ? ExpressionPromoter.Promote(expr1, expr2.Type, true, false) : null;
                Expression expr2As1 = expr1 != Constants.NullLiteral ? ExpressionPromoter.Promote(expr2, expr1.Type, true, false) : null;
                if (expr1As2 != null && expr2As1 == null)
                {
                    expr1 = expr1As2;
                }
                else if (expr2As1 != null && expr1As2 == null)
                {
                    expr2 = expr2As1;
                }
                else
                {
                    string type1 = expr1 != Constants.NullLiteral ? expr1.Type.Name : "null";
                    string type2 = expr2 != Constants.NullLiteral ? expr2.Type.Name : "null";
                    if (expr1As2 != null)
                    {
                        throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                    }

                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
                }
            }

            return Expression.Condition(test, expr1, expr2);
        }

        Expression ParseNew()
        {
            _textParser.NextToken();
            if (_textParser.CurrentToken.Id != TokenId.OpenParen &&
                _textParser.CurrentToken.Id != TokenId.OpenCurlyParen &&
                _textParser.CurrentToken.Id != TokenId.OpenBracket &&
                _textParser.CurrentToken.Id != TokenId.Identifier)
            {
                throw ParseError(Res.OpenParenOrIdentifierExpected);
            }

            Type newType = null;
            if (_textParser.CurrentToken.Id == TokenId.Identifier)
            {
                var newTypeName = _textParser.CurrentToken.Text;
                _textParser.NextToken();

                while (_textParser.CurrentToken.Id == TokenId.Dot || _textParser.CurrentToken.Id == TokenId.Plus)
                {
                    var sep = _textParser.CurrentToken.Text;
                    _textParser.NextToken();
                    if (_textParser.CurrentToken.Id != TokenId.Identifier)
                    {
                        throw ParseError(Res.IdentifierExpected);
                    }
                    newTypeName += sep + _textParser.CurrentToken.Text;
                    _textParser.NextToken();
                }

                newType = FindType(newTypeName);
                if (newType == null)
                {
                    throw ParseError(_textParser.CurrentToken.Pos, Res.TypeNotFound, newTypeName);
                }

                if (_textParser.CurrentToken.Id != TokenId.OpenParen &&
                    _textParser.CurrentToken.Id != TokenId.OpenBracket &&
                    _textParser.CurrentToken.Id != TokenId.OpenCurlyParen)
                {
                    throw ParseError(Res.OpenParenExpected);
                }
            }

            bool arrayInitializer = false;
            if (_textParser.CurrentToken.Id == TokenId.OpenBracket)
            {
                _textParser.NextToken();
                _textParser.ValidateToken(TokenId.CloseBracket, Res.CloseBracketExpected);
                _textParser.NextToken();
                _textParser.ValidateToken(TokenId.OpenCurlyParen, Res.OpenCurlyParenExpected);
                arrayInitializer = true;
            }

            _textParser.NextToken();

            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();

            while (_textParser.CurrentToken.Id != TokenId.CloseParen && _textParser.CurrentToken.Id != TokenId.CloseCurlyParen)
            {
                int exprPos = _textParser.CurrentToken.Pos;
                Expression expr = ParseConditionalOperator();
                if (!arrayInitializer)
                {
                    string propName;
                    if (TokenIdentifierIs("as"))
                    {
                        _textParser.NextToken();
                        propName = GetIdentifier();
                        _textParser.NextToken();
                    }
                    else
                    {
                        if (!TryGetMemberName(expr, out propName))
                        {
                            throw ParseError(exprPos, Res.MissingAsClause);
                        }
                    }

                    properties.Add(new DynamicProperty(propName, expr.Type));
                }

                expressions.Add(expr);

                if (_textParser.CurrentToken.Id != TokenId.Comma)
                {
                    break;
                }

                _textParser.NextToken();
            }

            if (_textParser.CurrentToken.Id != TokenId.CloseParen && _textParser.CurrentToken.Id != TokenId.CloseCurlyParen)
            {
                throw ParseError(Res.CloseParenOrCommaExpected);
            }
            _textParser.NextToken();

            if (arrayInitializer)
            {
                return CreateArrayInitializerExpression(expressions, newType);
            }

            return CreateNewExpression(properties, expressions, newType);
        }

        private Expression CreateArrayInitializerExpression(List<Expression> expressions, Type newType)
        {
            if (expressions.Count == 0)
            {
                return Expression.NewArrayInit(newType ?? typeof(object));
            }

            if (newType != null)
            {
                return Expression.NewArrayInit(
                    newType,
                    expressions.Select(expression => ExpressionPromoter.Promote(expression, newType, true, true)));
            }

            return Expression.NewArrayInit(
                expressions.All(expression => expression.Type == expressions[0].Type) ? expressions[0].Type : typeof(object),
                expressions);
        }

        private Expression CreateNewExpression(List<DynamicProperty> properties, List<Expression> expressions, Type newType)
        {
            // http://solutionizing.net/category/linq/
            Type type = newType ?? _resultType;

            if (type == null)
            {
#if !UAP10_0
                if (_parsingConfig != null && _parsingConfig.UseDynamicObjectClassForAnonymousTypes)
                {
#endif
                    type = typeof(DynamicClass);
                    Type typeForKeyValuePair = typeof(KeyValuePair<string, object>);
#if NET35 || NET40
                    ConstructorInfo constructorForKeyValuePair = typeForKeyValuePair.GetConstructors().First();
#else
                    ConstructorInfo constructorForKeyValuePair = typeForKeyValuePair.GetTypeInfo().DeclaredConstructors.First();
#endif
                    var arrayIndexParams = new List<Expression>();
                    for (int i = 0; i < expressions.Count; i++)
                    {
                        // Just convert the expression always to an object expression.
                        UnaryExpression boxingExpression = Expression.Convert(expressions[i], typeof(object));
                        NewExpression parameter = Expression.New(constructorForKeyValuePair, (Expression)Expression.Constant(properties[i].Name), boxingExpression);

                        arrayIndexParams.Add(parameter);
                    }

                    // Create an expression tree that represents creating and initializing a one-dimensional array of type KeyValuePair<string, object>.
                    NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof(KeyValuePair<string, object>), arrayIndexParams);

                    // Get the "public DynamicClass(KeyValuePair<string, object>[] propertylist)" constructor
#if NET35 || NET40
                    ConstructorInfo constructor = type.GetConstructors().First();
#else
                    ConstructorInfo constructor = type.GetTypeInfo().DeclaredConstructors.First();
#endif
                    return Expression.New(constructor, newArrayExpression);
#if !UAP10_0
                }

                type = DynamicClassFactory.CreateType(properties, _createParameterCtor);
#endif
            }

            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();
            if (type.GetTypeInfo().BaseType == typeof(DynamicClass))
            {
                propertyInfos = propertyInfos.Where(x => x.Name != "Item");
            }

            Type[] propertyTypes = propertyInfos.Select(p => p.PropertyType).ToArray();
            ConstructorInfo ctor = type.GetConstructor(propertyTypes);
            if (ctor != null && ctor.GetParameters().Length == expressions.Count)
            {
                return Expression.New(ctor, expressions, (IEnumerable<MemberInfo>)propertyInfos);
            }

            MemberBinding[] bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < bindings.Length; i++)
            {
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            }
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            int errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();
            Expression[] args = ParseArgumentList();
            if (MethodFinder.FindMethod(lambda.Type, "Invoke", false, args, out MethodBase _) != 1)
            {
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            }

            return Expression.Invoke(lambda, args);
        }

        Expression ParseTypeAccess(Type type)
        {
            int errorPos = _textParser.CurrentToken.Pos;
            _textParser.NextToken();

            if (_textParser.CurrentToken.Id == TokenId.Question)
            {
                if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
                {
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm, TypeHelper.GetTypeName(type));
                }

                type = typeof(Nullable<>).MakeGenericType(type);
                _textParser.NextToken();
            }

            // This is a shorthand for explicitely converting a string to something
            bool shorthand = _textParser.CurrentToken.Id == TokenId.StringLiteral;
            if (_textParser.CurrentToken.Id == TokenId.OpenParen || shorthand)
            {
                Expression[] args = shorthand ? new[] { ParseStringLiteral() } : ParseArgumentList();

                // If only 1 argument, and if the type is a ValueType and argType is also a ValueType, just Convert
                if (args.Length == 1)
                {
                    Type argType = args[0].Type;

                    if (type.GetTypeInfo().IsValueType && TypeHelper.IsNullableType(type) && argType.GetTypeInfo().IsValueType)
                    {
                        return Expression.Convert(args[0], type);
                    }
                }

                switch (MethodFinder.FindBestMethod(type.GetConstructors(), args, out MethodBase method))
                {
                    case 0:
                        if (args.Length == 1)
                        {
                            return GenerateConversion(args[0], type, errorPos);
                        }

                        throw ParseError(errorPos, Res.NoMatchingConstructor, TypeHelper.GetTypeName(type));

                    case 1:
                        return Expression.New((ConstructorInfo)method, args);

                    default:
                        throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, TypeHelper.GetTypeName(type));
                }
            }

            _textParser.ValidateToken(TokenId.Dot, Res.DotOrOpenParenOrStringLiteralExpected);
            _textParser.NextToken();

            return ParseMemberAccess(type, null);
        }

        static Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            Type exprType = expr.Type;
            if (exprType == type)
            {
                return expr;
            }

            if (exprType.GetTypeInfo().IsValueType && type.GetTypeInfo().IsValueType)
            {
                if ((TypeHelper.IsNullableType(exprType) || TypeHelper.IsNullableType(type)) && TypeHelper.GetNonNullableType(exprType) == TypeHelper.GetNonNullableType(type))
                {
                    return Expression.Convert(expr, type);
                }

                if ((TypeHelper.IsNumericType(exprType) || TypeHelper.IsEnumType(exprType)) && TypeHelper.IsNumericType(type) || TypeHelper.IsEnumType(type))
                {
                    return Expression.ConvertChecked(expr, type);
                }
            }

            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.GetTypeInfo().IsInterface || type.GetTypeInfo().IsInterface)
            {
                return Expression.Convert(expr, type);
            }

            // Try to Parse the string rather that just generate the convert statement
            if (expr.NodeType == ExpressionType.Constant && exprType == typeof(string))
            {
                string text = (string)((ConstantExpression)expr).Value;

                // DateTime is parsed as UTC time.
                if (type == typeof(DateTime) && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return Expression.Constant(dateTime, type);
                }

                object[] arguments = { text, null };
#if NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD
                MethodInfo method = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
#else
                MethodInfo method = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), type.MakeByRefType() }, null);
#endif
                if (method != null && (bool)method.Invoke(null, arguments))
                {
                    return Expression.Constant(arguments[1], type);
                }
            }

            throw ParseError(errorPos, Res.CannotConvertValue, TypeHelper.GetTypeName(exprType), TypeHelper.GetTypeName(type));
        }

        Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null)
            {
                type = instance.Type;
            }

            int errorPos = _textParser.CurrentToken.Pos;
            string id = GetIdentifier();
            _textParser.NextToken();

            if (_textParser.CurrentToken.Id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    Type enumerableType = TypeHelper.FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != null)
                    {
                        Type elementType = enumerableType.GetTypeInfo().GetGenericTypeArguments()[0];
                        return ParseAggregate(instance, elementType, id, errorPos, TypeHelper.FindGenericType(typeof(IQueryable<>), type) != null);
                    }
                }

                Expression[] args = ParseArgumentList();
                switch (MethodFinder.FindMethod(type, id, instance == null, args, out MethodBase mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableMethod, id, TypeHelper.GetTypeName(type));

                    case 1:
                        MethodInfo method = (MethodInfo)mb;
                        if (!_predefinedTypesHelper.IsPredefinedType(method.DeclaringType) && !(method.IsPublic && _predefinedTypesHelper.IsPredefinedType(method.ReturnType)))
                            throw ParseError(errorPos, Res.MethodsAreInaccessible, TypeHelper.GetTypeName(method.DeclaringType));

                        if (method.ReturnType == typeof(void))
                            throw ParseError(errorPos, Res.MethodIsVoid, id, TypeHelper.GetTypeName(method.DeclaringType));

                        return Expression.Call(instance, method, args);

                    default:
                        throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, TypeHelper.GetTypeName(type));
                }
            }

            if (type.GetTypeInfo().IsEnum)
            {
                var @enum = Enum.Parse(type, id, true);

                return Expression.Constant(@enum);
            }

#if NETFX_CORE
            if (type == typeof(DynamicObjectClass))
            {
                return Expression.MakeIndex(instance, typeof(DynamicObjectClass).GetProperty("Item"), new[] { Expression.Constant(id) });
            }
#endif
            MemberInfo member = FindPropertyOrField(type, id, instance == null);
            if (member == null)
            {
                if (_textParser.CurrentToken.Id == TokenId.Lambda && _it.Type == type)
                {
                    // This might be an internal variable for use within a lambda expression, so store it as such
                    _internals.Add(id, _it);
                    _textParser.NextToken();

                    return ParseConditionalOperator();
                }

                throw ParseError(errorPos, Res.UnknownPropertyOrField, id, TypeHelper.GetTypeName(type));
            }

            var property = member as PropertyInfo;
            if (property != null)
            {
                return Expression.Property(instance, property);
            }

            return Expression.Field(instance, (FieldInfo)member);
        }

        Type FindType(string name)
        {
            _keywordsHelper.TryGetValue(name, out object type);

            var result = type as Type;
            if (result != null)
            {
                return result;
            }
            if (_it != null && _it.Type.Name == name)
            {
                return _it.Type;
            }
            if (_parent != null && _parent.Type.Name == name)
            {
                return _parent.Type;
            }
            if (_root != null && _root.Type.Name == name)
            {
                return _root.Type;
            }
            if (_it != null && _it.Type.Namespace + "." + _it.Type.Name == name)
            {
                return _it.Type;
            }
            if (_parent != null && _parent.Type.Namespace + "." + _parent.Type.Name == name)
            {
                return _parent.Type;
            }
            if (_root != null && _root.Type.Namespace + "." + _root.Type.Name == name)
            {
                return _root.Type;
            }

            return null;
        }

        Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos, bool isQueryable)
        {
            var oldParent = _parent;

            ParameterExpression outerIt = _it;
            ParameterExpression innerIt = Expression.Parameter(elementType, "");

            _parent = _it;

            if (methodName == "Contains" || methodName == "Skip" || methodName == "Take")
            {
                // for any method that acts on the parent element type, we need to specify the outerIt as scope.
                _it = outerIt;
            }
            else
            {
                _it = innerIt;
            }

            Expression[] args = ParseArgumentList();

            _it = outerIt;
            _parent = oldParent;

            if (!MethodFinder.ContainsMethod(typeof(IEnumerableSignatures), methodName, false, args))
            {
                throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);
            }

            Type callType = typeof(Enumerable);
            if (isQueryable && MethodFinder.ContainsMethod(typeof(IQueryableSignatures), methodName, false, args))
            {
                callType = typeof(Queryable);
            }

            Type[] typeArgs;
            if (new[] { "Min", "Max", "Select", "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending", "GroupBy" }.Contains(methodName))
            {
                if (args.Length == 2)
                {
                    typeArgs = new[] { elementType, args[0].Type, args[1].Type };
                }
                else
                {
                    typeArgs = new[] { elementType, args[0].Type };
                }
            }
            else if (methodName == "SelectMany")
            {
                var type = Expression.Lambda(args[0], innerIt).Body.Type;
                var interfaces = type.GetInterfaces().Union(new[] { type });
                Type interfaceType = interfaces.Single(i => i.Name == typeof(IEnumerable<>).Name);
                Type resultType = interfaceType.GetTypeInfo().GetGenericTypeArguments()[0];
                typeArgs = new[] { elementType, resultType };
            }
            else
            {
                typeArgs = new[] { elementType };
            }

            if (args.Length == 0)
            {
                args = new[] { instance };
            }
            else
            {
                if (new[] { "Contains", "Take", "Skip", "DefaultIfEmpty" }.Contains(methodName))
                {
                    args = new[] { instance, args[0] };
                }
                else
                {
                    if (args.Length == 2)
                    {
                        args = new[] { instance, Expression.Lambda(args[0], innerIt), Expression.Lambda(args[1], innerIt) };
                    }
                    else
                    {
                        args = new[] { instance, Expression.Lambda(args[0], innerIt) };
                    }
                }
            }

            return Expression.Call(callType, methodName, typeArgs, args);
        }

        Expression[] ParseArgumentList()
        {
            _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            _textParser.NextToken();
            Expression[] args = _textParser.CurrentToken.Id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
            _textParser.ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            _textParser.NextToken();
            return args;
        }

        Expression[] ParseArguments()
        {
            var argList = new List<Expression>();
            while (true)
            {
                argList.Add(ParseConditionalOperator());

                if (_textParser.CurrentToken.Id != TokenId.Comma)
                {
                    break;
                }

                _textParser.NextToken();
            }

            return argList.ToArray();
        }

        Expression ParseElementAccess(Expression expr)
        {
            int errorPos = _textParser.CurrentToken.Pos;
            _textParser.ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            _textParser.NextToken();

            Expression[] args = ParseArguments();
            _textParser.ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            _textParser.NextToken();

            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                {
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                }
                Expression index = ExpressionPromoter.Promote(args[0], typeof(int), true, false);

                if (index == null)
                {
                    throw ParseError(errorPos, Res.InvalidIndex);
                }

                return Expression.ArrayIndex(expr, index);
            }

            switch (MethodFinder.FindIndexer(expr.Type, args, out var mb))
            {
                case 0:
                    throw ParseError(errorPos, Res.NoApplicableIndexer,
                        TypeHelper.GetTypeName(expr.Type));
                case 1:
                    return Expression.Call(expr, (MethodInfo)mb, args);

                default:
                    throw ParseError(errorPos, Res.AmbiguousIndexerInvocation, TypeHelper.GetTypeName(expr.Type));
            }
        }

        public static Type ToNullableType(Type type)
        {
            Check.NotNull(type, nameof(type));

            if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
            {
                throw ParseError(-1, Res.TypeHasNoNullableForm, TypeHelper.GetTypeName(type));
            }

            return typeof(Nullable<>).MakeGenericType(type);
        }

        static bool TryGetMemberName(Expression expression, out string memberName)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null && expression.NodeType == ExpressionType.Coalesce)
            {
                memberExpression = (expression as BinaryExpression).Left as MemberExpression;
            }

            if (memberExpression != null)
            {
                memberName = memberExpression.Member.Name;
                return true;
            }

#if NETFX_CORE
            var indexExpression = expression as IndexExpression;
            if (indexExpression != null && indexExpression.Indexer.DeclaringType == typeof(DynamicObjectClass))
            {
                memberName = ((ConstantExpression)indexExpression.Arguments.First()).Value as string;
                return true;
            }
#endif
            memberName = null;
            return false;
        }

        void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            Expression[] args = { expr };

            if (!MethodFinder.ContainsMethod(signatures, "F", false, args))
            {
                throw IncompatibleOperandError(opName, expr, errorPos);
            }

            expr = args[0];
        }

        void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            Expression[] args = { left, right };

            if (!MethodFinder.ContainsMethod(signatures, "F", false, args))
            {
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            }

            left = args[0];
            right = args[1];
        }

        static Exception IncompatibleOperandError(string opName, Expression expr, int errorPos)
        {
            return ParseError(errorPos, Res.IncompatibleOperand, opName, TypeHelper.GetTypeName(expr.Type));
        }

        static Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int errorPos)
        {
            return ParseError(errorPos, Res.IncompatibleOperands, opName, TypeHelper.GetTypeName(left.Type), TypeHelper.GetTypeName(right.Type));
        }

        static MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in TypeHelper.GetSelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0)
                {
                    return members[0];
                }
            }
            return null;
#else
            foreach (Type t in TypeHelper.GetSelfAndBaseTypes(type))
            {
                // Try to find a property with the specified memberName
                MemberInfo member = t.GetTypeInfo().DeclaredProperties.FirstOrDefault(x => x.Name.ToLowerInvariant() == memberName.ToLowerInvariant());
                if (member != null)
                {
                    return member;
                }

                // If no property is found, try to get a field with the specified memberName
                member = t.GetTypeInfo().DeclaredFields.FirstOrDefault(x => (x.IsStatic || !staticAccess) && x.Name.ToLowerInvariant() == memberName.ToLowerInvariant());
                if (member != null)
                {
                    return member;
                }

                // No property or field is found, try the base type.
            }
            return null;
#endif
        }

        bool TokenIdentifierIs(string id)
        {
            return _textParser.CurrentToken.Id == TokenId.Identifier && string.Equals(id, _textParser.CurrentToken.Text, StringComparison.OrdinalIgnoreCase);
        }

        string GetIdentifier()
        {
            _textParser.ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            string id = _textParser.CurrentToken.Text;
            if (id.Length > 1 && id[0] == '@')
            {
                id = id.Substring(1);
            }

            return id;
        }

        Exception ParseError(string format, params object[] args)
        {
            return ParseError(_textParser?.CurrentToken.Pos ?? 0, format, args);
        }

        static Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }
    }
}
