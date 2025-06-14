﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Dynamic.Core.Config;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Extensions;
using System.Linq.Dynamic.Core.Parser.SupportedMethods;
using System.Linq.Dynamic.Core.Parser.SupportedOperands;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Linq.Dynamic.Core.TypeConverters;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;
using AnyOfTypes;

namespace System.Linq.Dynamic.Core.Parser;

/// <summary>
/// ExpressionParser
/// </summary>
public class ExpressionParser
{
    private static readonly string[] OutKeywords = ["out", "$out"];
    private const string DiscardVariable = "_";

    private const string MethodOrderBy = nameof(Queryable.OrderBy);
    private const string MethodOrderByDescending = nameof(Queryable.OrderByDescending);
    private const string MethodThenBy = nameof(Queryable.ThenBy);
    private const string MethodThenByDescending = nameof(Queryable.ThenByDescending);

    private readonly ParsingConfig _parsingConfig;
    private readonly MethodFinder _methodFinder;
    private readonly IKeywordsHelper _keywordsHelper;
    private readonly TextParser _textParser;
    private readonly NumberParser _numberParser;
    private readonly IExpressionHelper _expressionHelper;
    private readonly ConstantExpressionHelper _constantExpressionHelper;
    private readonly ITypeFinder _typeFinder;
    private readonly ITypeConverterFactory _typeConverterFactory;
    private readonly Dictionary<string, object> _internals = new();
    private readonly Dictionary<string, object?> _symbols;
    private readonly bool _usedForOrderBy;

    private IDictionary<string, object>? _externals;
    private ParameterExpression? _it;
    private ParameterExpression? _parent;
    private ParameterExpression? _root;
    private Type? _resultType;
    private bool _createParameterCtor;

    /// <summary>
    /// Gets name for the `it` field. By default this is set to the KeyWord value "it".
    /// </summary>
    public string ItName { get; private set; } = KeywordsHelper.KEYWORD_IT;

    /// <summary>
    /// There was a problem when an expression contained multiple lambdas where the ItName was not cleared and freed for the next lambda.
    /// This variable stores the ItName of the last parsed lambda.
    /// Not used internally by ExpressionParser, but used to preserve compatibility of parsingConfig.RenameParameterExpression
    /// which was designed to only work with mono-lambda expressions.
    /// </summary>
    public string LastLambdaItName { get; private set; } = KeywordsHelper.KEYWORD_IT;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="values">The values.</param>
    /// <param name="parsingConfig">The parsing configuration.</param>
    public ExpressionParser(ParameterExpression[]? parameters, string expression, object?[]? values, ParsingConfig? parsingConfig) :
        this(parameters, expression, values, parsingConfig, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionParser"/> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="values">The values.</param>
    /// <param name="parsingConfig">The parsing configuration.</param>
    /// <param name="usedForOrderBy">Indicate that this instance will be used for parsing orderBy. Default value is <c>false</c>.</param>
    public ExpressionParser(ParameterExpression[]? parameters, string expression, object?[]? values, ParsingConfig? parsingConfig, bool usedForOrderBy)
    {
        Check.NotEmpty(expression);

        _usedForOrderBy = usedForOrderBy;
        _symbols = new Dictionary<string, object?>(parsingConfig is { IsCaseSensitive: true } ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        _parsingConfig = parsingConfig ?? ParsingConfig.Default;

        _keywordsHelper = new KeywordsHelper(_parsingConfig);
        _textParser = new TextParser(_parsingConfig, expression);
        _numberParser = new NumberParser(parsingConfig);
        _expressionHelper = new ExpressionHelper(_parsingConfig);
        _methodFinder = new MethodFinder(_parsingConfig, _expressionHelper);
        _typeFinder = new TypeFinder(_parsingConfig, _keywordsHelper);
        _typeConverterFactory = new TypeConverterFactory(_parsingConfig);
        _constantExpressionHelper = ConstantExpressionHelperFactory.GetInstance(_parsingConfig);

        if (parameters != null)
        {
            ProcessParameters(parameters);
        }

        if (values != null)
        {
            ProcessValues(values);
        }
    }

    private void ProcessParameters(ParameterExpression[] parameters)
    {
        foreach (var pe in parameters.Where(p => !string.IsNullOrEmpty(p.Name)))
        {
            AddSymbol(pe.Name!, pe);
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

    private void ProcessValues(object?[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            object? value = values[i];
            IDictionary<string, object>? externals;

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

    private void AddSymbol(string name, object? value)
    {
        if (_symbols.ContainsKey(name))
        {
            throw ParseError(Res.DuplicateIdentifier, name);
        }

        _symbols.Add(name, value);
    }

    /// <summary>
    /// Uses the TextParser to parse the string into the specified result type.
    /// </summary>
    /// <param name="resultType">Type of the result.</param>
    /// <param name="createParameterCtor">if set to <c>true</c> [create parameter ctor].</param>
    /// <returns>Expression</returns>
    public Expression Parse(Type? resultType, bool createParameterCtor = true)
    {
        _resultType = resultType;
        _createParameterCtor = createParameterCtor;

        int exprPos = _textParser.CurrentToken.Pos;
        Expression? expr = ParseConditionalOperator();

        if (resultType != null)
        {
            if ((expr = _parsingConfig.ExpressionPromoter.Promote(expr, resultType, true, false)) == null)
            {
                throw ParseError(exprPos, Res.ExpressionTypeMismatch, TypeHelper.GetTypeName(resultType));
            }
        }

        _textParser.ValidateToken(TokenId.End, Res.SyntaxError);

        return expr;
    }

    // out keyword
    private Expression ParseOutKeyword()
    {
        if (_textParser.CurrentToken.Id == TokenId.Identifier && OutKeywords.Contains(_textParser.CurrentToken.Text))
        {
            // Go to next token (which should be a '_')
            _textParser.NextToken();

            var variableName = _textParser.CurrentToken.Text;
            if (variableName != DiscardVariable)
            {
                throw ParseError(_textParser.CurrentToken.Pos, Res.OutKeywordRequiresDiscard);
            }

            // Advance to next token
            _textParser.NextToken();

            // Use MakeByRefType() to indicate that it's a by-reference type because C# uses this for both 'ref' and 'out' parameters.
            // The "typeof(object).MakeByRefType()" is used, this will be changed later in the flow to the real type.
            return Expression.Parameter(typeof(object).MakeByRefType(), variableName);
        }

        return ParseConditionalOperator();
    }

    internal IList<DynamicOrdering> ParseOrdering(bool forceThenBy = false)
    {
        var orderings = new List<DynamicOrdering>();
        while (true)
        {
            var expr = ParseConditionalOperator();
            var ascending = true;
            if (TokenIsIdentifier("asc") || TokenIsIdentifier("ascending"))
            {
                _textParser.NextToken();
            }
            else if (TokenIsIdentifier("desc") || TokenIsIdentifier("descending"))
            {
                _textParser.NextToken();
                ascending = false;
            }

            string methodName;
            if (forceThenBy || orderings.Count > 0)
            {
                methodName = ascending ? MethodThenBy : MethodThenByDescending;
            }
            else
            {
                methodName = ascending ? MethodOrderBy : MethodOrderByDescending;
            }

            orderings.Add(new DynamicOrdering(expr, ascending, methodName));

            if (_textParser.CurrentToken.Id != TokenId.Comma)
            {
                break;
            }

            _textParser.NextToken();
        }

        _textParser.ValidateToken(TokenId.End, Res.SyntaxError);
        return orderings;
    }

    // ?: operator
    private Expression ParseConditionalOperator()
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
            expr = GenerateConditional(expr, expr1, expr2, false, errorPos);
        }
        return expr;
    }

    // ?? (null-coalescing) operator
    private Expression ParseNullCoalescingOperator()
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
    private Expression ParseLambdaOperator()
    {
        var expr = ParseOrOperator();

        if (_textParser.CurrentToken.Id == TokenId.Lambda && _it?.Type == expr.Type)
        {
            _textParser.NextToken();
            if (_textParser.CurrentToken.Id is TokenId.Identifier or TokenId.OpenParen)
            {
                var right = ParseConditionalOperator();
                return Expression.Lambda(right, new[] { (ParameterExpression)expr });
            }
            _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
        }

        return expr;
    }

    // Or operator
    // - ||
    // - Or
    // - OrElse
    private Expression ParseOrOperator()
    {
        Expression left = ParseAndOperator();
        while (_textParser.CurrentToken.Id == TokenId.DoubleBar)
        {
            Token op = _textParser.CurrentToken;
            _textParser.NextToken();
            Expression right = ParseAndOperator();
            CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
            left = Expression.OrElse(left, right);
        }
        return left;
    }

    // And operator
    // - &&
    // - And
    // - AndAlso
    private Expression ParseAndOperator()
    {
        Expression left = ParseIn();
        while (_textParser.CurrentToken.Id == TokenId.DoubleAmpersand)
        {
            Token op = _textParser.CurrentToken;
            _textParser.NextToken();
            Expression right = ParseIn();
            CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
            left = Expression.AndAlso(left, right);
        }
        return left;
    }

    // "in" / "not in" / "not_in" operator for literals - example: "x in (1,2,3,4)"
    // "in" / "not in" / "not_in" operator to mimic contains - example: "x in @0", compare to @0.Contains(x)
    private Expression ParseIn()
    {
        Expression left = ParseLogicalAndOrOperator();
        Expression accumulate = left;

        while (_textParser.TryGetToken(["in", "not_in", "not"], [TokenId.Exclamation], out var token))
        {
            var not = false;
            if (token.Text == "not_in")
            {
                not = true;
            }
            else if (token.Text == "not" || token.Id == TokenId.Exclamation)
            {
                not = true;

                _textParser.NextToken();

                if (!TokenIsIdentifier("in"))
                {
                    throw ParseError(token.Pos, Res.TokenExpected, "in");
                }
            }

            _textParser.NextToken();

            if (_textParser.CurrentToken.Id == TokenId.OpenParen) // literals (or other inline list)
            {
                while (_textParser.CurrentToken.Id != TokenId.CloseParen)
                {
                    _textParser.NextToken();

                    // we need to parse unary expressions because otherwise 'in' clause will fail in use cases like 'in (-1, -1)' or 'in (!true)'
                    Expression right = ParseUnary();

                    // if the identifier is an Enum (or nullable Enum), try to convert the right-side also to an Enum.
                    if (TypeHelper.GetNonNullableType(left.Type).GetTypeInfo().IsEnum)
                    {
                        if (right is ConstantExpression constantExprRight)
                        {
                            right = ParseEnumToConstantExpression(token.Pos, left.Type, constantExprRight);
                        }
                        else if (_expressionHelper.TryUnwrapAsConstantExpression(right, out var unwrappedConstantExprRight))
                        {
                            right = ParseEnumToConstantExpression(token.Pos, left.Type, unwrappedConstantExprRight);
                        }
                    }

                    // else, check for direct type match
                    else if (left.Type != right.Type)
                    {
                        CheckAndPromoteOperands(typeof(IEqualitySignatures), TokenId.DoubleEqual, "==", ref left, ref right, token.Pos);
                    }

                    if (accumulate.Type != typeof(bool))
                    {
                        accumulate = _expressionHelper.GenerateEqual(left, right);
                    }
                    else
                    {
                        accumulate = Expression.OrElse(accumulate, _expressionHelper.GenerateEqual(left, right));
                    }

                    if (_textParser.CurrentToken.Id == TokenId.End)
                    {
                        throw ParseError(token.Pos, Res.CloseParenOrCommaExpected);
                    }
                }

                // Since this started with an open paren, make sure to move off the close
                _textParser.NextToken();
            }
            else if (_textParser.CurrentToken.Id == TokenId.Identifier) // a single argument
            {
                Expression right = ParsePrimary();

                if (!typeof(IEnumerable).IsAssignableFrom(right.Type))
                {
                    throw ParseError(_textParser.CurrentToken.Pos, Res.IdentifierImplementingInterfaceExpected, typeof(IEnumerable));
                }

                var typeArgs = new[] { left.Type };

                var args = new[] { right, left };

                accumulate = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), typeArgs, args);
            }
            else
            {
                throw ParseError(token.Pos, Res.OpenParenOrIdentifierExpected);
            }

            if (not)
            {
                accumulate = Expression.Not(accumulate);
            }
        }

        return accumulate;
    }

    // &, | bitwise operators
    private Expression ParseLogicalAndOrOperator()
    {
        Expression left = ParseComparisonOperator();

        var leftType = left.Type;
        var typesAreSame = true;
        var numberOfArguments = 0;

        while (_textParser.CurrentToken.Id is TokenId.Ampersand or TokenId.Bar)
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
                case TokenId.Ampersand:
                    if (left.Type == typeof(string) && left.NodeType == ExpressionType.Constant && int.TryParse((string?)((ConstantExpression)left).Value, out var parseValue) && TypeHelper.IsNumericType(right.Type))
                    {
                        left = Expression.Constant(parseValue);
                    }
                    else if (right.Type == typeof(string) && right.NodeType == ExpressionType.Constant && int.TryParse((string?)((ConstantExpression)right).Value, out parseValue) && TypeHelper.IsNumericType(left.Type))
                    {
                        right = Expression.Constant(parseValue);
                    }

                    // When at least one side of the operator is a string, consider it's a VB-style concatenation operator.
                    // Doesn't break any other function since logical AND with a string is invalid anyway.
                    if (left.Type == typeof(string) || right.Type == typeof(string))
                    {
                        left = _expressionHelper.GenerateStringConcat(left, right);
                    }
                    else
                    {
                        typesAreSame &= _expressionHelper.ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref left, ref right);
                        left = Expression.And(left, right);
                    }
                    break;

                case TokenId.Bar:
                    typesAreSame &= _expressionHelper.ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref left, ref right);
                    left = Expression.Or(left, right);
                    break;
            }

            numberOfArguments++;
        }

        // #695
        if (numberOfArguments > 0 && typesAreSame && leftType.GetTypeInfo().IsEnum)
        {
            return Expression.Convert(left, leftType);
        }

        return left;
    }

    // =, ==, !=, <>, >, >=, <, <= operators
    private Expression ParseComparisonOperator()
    {
        Expression left = ParseShiftOperator();
        while (_textParser.CurrentToken.Id.IsComparisonOperator())
        {
            ConstantExpression? constantExpr;
            TypeConverter typeConverter;
            Token op = _textParser.CurrentToken;
            _textParser.NextToken();
            Expression right = ParseShiftOperator();

            var isEquality = op.Id.IsEqualityOperator();

            if (isEquality && (!left.Type.GetTypeInfo().IsValueType && !right.Type.GetTypeInfo().IsValueType || left.Type == typeof(Guid) && right.Type == typeof(Guid)))
            {
                // If left or right is NullLiteral, just continue. Else check if the types differ.
                if (!(Constants.IsNull(left) || Constants.IsNull(right)) && left.Type != right.Type)
                {
                    if (left.Type.IsAssignableFrom(right.Type) || HasImplicitConversion(right.Type, left.Type))
                    {
                        right = Expression.Convert(right, left.Type);
                    }
                    else if (right.Type.IsAssignableFrom(left.Type) || HasImplicitConversion(left.Type, right.Type))
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
                    Expression? e;
                    if ((e = _parsingConfig.ExpressionPromoter.Promote(right, left.Type, true, false)) != null)
                    {
                        right = e;
                    }
                    else if ((e = _parsingConfig.ExpressionPromoter.Promote(left, right.Type, true, false)) != null)
                    {
                        left = e;
                    }
                    else if (TypeHelper.IsEnumType(left.Type))
                    {
                        if (right is ConstantExpression constantExprRight)
                        {
                            right = ParseEnumToConstantExpression(op.Pos, left.Type, constantExprRight);
                        }
                        else if (_expressionHelper.TryUnwrapAsConstantExpression(right, out var unwrappedConstantExprRight))
                        {
                            right = ParseEnumToConstantExpression(op.Pos, left.Type, unwrappedConstantExprRight);
                        }
                    }
                    else if (TypeHelper.IsEnumType(right.Type))
                    {
                        if (left is ConstantExpression constantExprLeft)
                        {
                            left = ParseEnumToConstantExpression(op.Pos, right.Type, constantExprLeft);
                        }
                        else if (_expressionHelper.TryUnwrapAsConstantExpression(left, out var unwrappedConstantExprLeft))
                        {
                            left = ParseEnumToConstantExpression(op.Pos, right.Type, unwrappedConstantExprLeft);
                        }
                    }
                    else
                    {
                        throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                    }
                }
            }
            else if ((constantExpr = right as ConstantExpression) != null && constantExpr.Value is string stringValueR && (typeConverter = _typeConverterFactory.GetConverter(left.Type)) != null && typeConverter.CanConvertFrom(right.Type))
            {
                right = Expression.Constant(typeConverter.ConvertFromInvariantString(stringValueR), left.Type);
            }
            else if ((constantExpr = left as ConstantExpression) != null && constantExpr.Value is string stringValueL && (typeConverter = _typeConverterFactory.GetConverter(right.Type)) != null && typeConverter.CanConvertFrom(left.Type))
            {
                left = Expression.Constant(typeConverter.ConvertFromInvariantString(stringValueL), right.Type);
            }
            else if (_expressionHelper.TryUnwrapAsValue<string>(right, out var unwrappedStringValueR) && (typeConverter = _typeConverterFactory.GetConverter(left.Type)) != null && typeConverter.CanConvertFrom(right.Type))
            {
                right = Expression.Constant(typeConverter.ConvertFromInvariantString(unwrappedStringValueR), left.Type);
            }
            else if (_expressionHelper.TryUnwrapAsValue<string>(left, out var unwrappedStringValueL) && (typeConverter = _typeConverterFactory.GetConverter(right.Type)) != null && typeConverter.CanConvertFrom(left.Type))
            {
                left = Expression.Constant(typeConverter.ConvertFromInvariantString(unwrappedStringValueL), right.Type);
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
                    if ((TypeHelper.IsClass(left.Type) || TypeHelper.IsStruct(left.Type)) && right is ConstantExpression)
                    {
                        if (HasImplicitConversion(left.Type, right.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else if (HasImplicitConversion(right.Type, left.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                    }
                    else if ((TypeHelper.IsClass(right.Type) || TypeHelper.IsStruct(right.Type)) && left is ConstantExpression)
                    {
                        if (HasImplicitConversion(right.Type, left.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (HasImplicitConversion(left.Type, right.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                    }
                    else
                    {
                        CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                    }
                }
            }

            switch (op.Id)
            {
                case TokenId.Equal:
                case TokenId.DoubleEqual:
                    left = _expressionHelper.GenerateEqual(left, right);
                    break;
                case TokenId.ExclamationEqual:
                case TokenId.LessGreater:
                    left = _expressionHelper.GenerateNotEqual(left, right);
                    break;
                case TokenId.GreaterThan:
                    left = _expressionHelper.GenerateGreaterThan(left, right);
                    break;
                case TokenId.GreaterThanEqual:
                    left = _expressionHelper.GenerateGreaterThanEqual(left, right);
                    break;
                case TokenId.LessThan:
                    left = _expressionHelper.GenerateLessThan(left, right);
                    break;
                case TokenId.LessThanEqual:
                    left = _expressionHelper.GenerateLessThanEqual(left, right);
                    break;
            }
        }

        return left;
    }

    private static bool HasImplicitConversion(Type baseType, Type targetType)
    {
        var baseTypeHasConversion = baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
            .Any(mi => mi.GetParameters().FirstOrDefault()?.ParameterType == baseType);

        if (baseTypeHasConversion)
        {
            return true;
        }

        return targetType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
            .Any(mi => mi.GetParameters().FirstOrDefault()?.ParameterType == baseType);
    }

    private static ConstantExpression ParseEnumToConstantExpression(int pos, Type leftType, ConstantExpression constantExpr)
    {
        return Expression.Constant(ParseConstantExpressionToEnum(pos, leftType, constantExpr), leftType);
    }

    private static object ParseConstantExpressionToEnum(int pos, Type leftType, ConstantExpression constantExpr)
    {
        try
        {
            if (constantExpr.Value is string stringValue)
            {
                return Enum.Parse(TypeHelper.GetNonNullableType(leftType), stringValue, true);
            }
        }
        catch
        {
            throw ParseError(pos, Res.ExpressionTypeMismatch, leftType);
        }

        try
        {
            return Enum.ToObject(TypeHelper.GetNonNullableType(leftType), constantExpr.Value!);
        }
        catch
        {
            throw ParseError(pos, Res.ExpressionTypeMismatch, leftType);
        }
    }

    // <<, >> operators
    private Expression ParseShiftOperator()
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
                    CheckAndPromoteOperands(typeof(IShiftSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                    left = Expression.LeftShift(left, right);
                    break;
                case TokenId.DoubleGreaterThan:
                    CheckAndPromoteOperands(typeof(IShiftSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                    left = Expression.RightShift(left, right);
                    break;
            }
        }
        return left;
    }

    // +, - operators
    private Expression ParseAdditive()
    {
        Expression left = ParseArithmetic();
        while (_textParser.CurrentToken.Id is TokenId.Plus or TokenId.Minus)
        {
            Token op = _textParser.CurrentToken;
            _textParser.NextToken();

            Expression right = ParseArithmetic();
            switch (op.Id)
            {
                case TokenId.Plus:
                    if (left.Type == typeof(string) || right.Type == typeof(string))
                    {
                        left = _expressionHelper.GenerateStringConcat(left, right);
                    }
                    else
                    {
                        CheckAndPromoteOperands(typeof(IAddSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                        left = _expressionHelper.GenerateAdd(left, right);
                    }
                    break;

                case TokenId.Minus:
                    CheckAndPromoteOperands(typeof(ISubtractSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
                    left = _expressionHelper.GenerateSubtract(left, right);
                    break;
            }
        }
        return left;
    }

    // *, /, %, mod operators
    private Expression ParseArithmetic()
    {
        Expression left = ParseUnary();
        while (_textParser.CurrentToken.Id is TokenId.Asterisk or TokenId.Slash or TokenId.Percent || TokenIsIdentifier("mod"))
        {
            Token op = _textParser.CurrentToken;
            _textParser.NextToken();
            Expression right = ParseUnary();
            CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.Id, op.Text, ref left, ref right, op.Pos);
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
    private Expression ParseUnary()
    {
        if (_textParser.CurrentToken.Id == TokenId.Minus || _textParser.CurrentToken.Id == TokenId.Exclamation || TokenIsIdentifier("not"))
        {
            Token op = _textParser.CurrentToken;
            _textParser.NextToken();
            if (op.Id == TokenId.Minus && _textParser.CurrentToken.Id is TokenId.IntegerLiteral or TokenId.RealLiteral)
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

    private Expression ParsePrimary()
    {
        var expr = ParsePrimaryStart();
        _expressionHelper.WrapConstantExpression(ref expr);

        while (true)
        {
            if (_textParser.CurrentToken.Id == TokenId.Dot)
            {
                _textParser.NextToken();
                expr = ParseMemberAccess(null, expr);
            }
            else if (_textParser.CurrentToken.Id == TokenId.NullPropagation)
            {
                throw new NotSupportedException("An expression tree lambda may not contain a null propagating operator. Use the 'np()' or 'np(...)' (null-propagation) function instead.");
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

    private Expression ParsePrimaryStart()
    {
        switch (_textParser.CurrentToken.Id)
        {
            case TokenId.Identifier:
                return ParseIdentifier();

            case TokenId.StringLiteral:
                return ParseStringLiteralAsStringExpressionOrTypeExpression();

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

    private Expression ParseStringLiteralAsStringExpressionOrTypeExpression()
    {
        var clonedTextParser = _textParser.Clone();
        clonedTextParser.NextToken();

        // Check if next token is a "(" or a "?(".
        // Used for casting like $"\"System.DateTime\"(Abc)" or $"\"System.DateTime\"?(Abc)".
        // In that case, the string value is NOT forced to stay a string.
        bool forceParseAsString = true;
        if (clonedTextParser.CurrentToken.Id == TokenId.OpenParen)
        {
            forceParseAsString = false;
        }
        else if (clonedTextParser.CurrentToken.Id == TokenId.Question)
        {
            clonedTextParser.NextToken();
            if (clonedTextParser.CurrentToken.Id == TokenId.OpenParen)
            {
                forceParseAsString = false;
            }
        }

        var expressionOrType = ParseStringLiteral(forceParseAsString);
        return expressionOrType.IsFirst ? expressionOrType.First : ParseTypeAccess(expressionOrType.Second, false);
    }

    private AnyOf<Expression, Type> ParseStringLiteral(bool forceParseAsString)
    {
        _textParser.ValidateToken(TokenId.StringLiteral);

        var text = _textParser.CurrentToken.Text;
        var parsedStringValue = ParseStringAndEscape(text);

        if (_textParser.CurrentToken.Text[0] == '\'')
        {
            if (parsedStringValue.Length > 1)
            {
                throw ParseError(Res.InvalidCharacterLiteral);
            }

            _textParser.NextToken();
            return _constantExpressionHelper.CreateLiteral(parsedStringValue[0], parsedStringValue);
        }

        _textParser.NextToken();

        if (_parsingConfig.SupportCastingToFullyQualifiedTypeAsString && !forceParseAsString && parsedStringValue.Length > 2 && parsedStringValue.Contains('.'))
        {
            // Try to resolve this string as a type
            if (_typeFinder.TryFindTypeByName(parsedStringValue, null, false, out var type))
            {
                return type;
            }
        }

        // While the next token is also a string, keep concatenating these strings and get next token
        while (_textParser.CurrentToken.Id == TokenId.StringLiteral)
        {
            text += _textParser.CurrentToken.Text;
            _textParser.NextToken();
        }

        parsedStringValue = ParseStringAndEscape(text);

        return _constantExpressionHelper.CreateLiteral(parsedStringValue, parsedStringValue);
    }

    private string ParseStringAndEscape(string text)
    {
        return _parsingConfig.StringLiteralParsing == StringLiteralParsingType.EscapeDoubleQuoteByTwoDoubleQuotes ?
            StringParser.ParseStringAndUnescapeTwoDoubleQuotesByASingleDoubleQuote(text, _textParser.CurrentToken.Pos) :
            StringParser.ParseStringAndUnescape(text, _textParser.CurrentToken.Pos);
    }

    private Expression ParseIntegerLiteral()
    {
        _textParser.ValidateToken(TokenId.IntegerLiteral);

        string text = _textParser.CurrentToken.Text;

        var tokenPosition = _textParser.CurrentToken.Pos;

        var constantExpression = _numberParser.ParseIntegerLiteral(tokenPosition, text);
        _textParser.NextToken();
        return constantExpression;
    }

    private Expression ParseRealLiteral()
    {
        _textParser.ValidateToken(TokenId.RealLiteral);

        string text = _textParser.CurrentToken.Text;

        _textParser.NextToken();

        return _numberParser.ParseRealLiteral(text, text[text.Length - 1], true);
    }

    private Expression ParseParenExpression()
    {
        _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
        _textParser.NextToken();
        Expression e = ParseConditionalOperator();
        _textParser.ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
        _textParser.NextToken();
        return e;
    }

    private Expression ParseIdentifier()
    {
        _textParser.ValidateToken(TokenId.Identifier);

        var isValid = _keywordsHelper.TryGetValue(_textParser.CurrentToken.Text, out var keywordOrType);
        var shouldPrioritizeType = true;

        if (_parsingConfig.PrioritizePropertyOrFieldOverTheType && keywordOrType.IsThird)
        {
            var isPropertyOrField = _it != null && FindPropertyOrField(_it.Type, _textParser.CurrentToken.Text, false) != null;
            var hasSymbol = _symbols.ContainsKey(_textParser.CurrentToken.Text);
            if (isPropertyOrField || hasSymbol)
            {
                shouldPrioritizeType = false;
            }
        }

        if (isValid && shouldPrioritizeType)
        {
            var keywordOrFunctionAllowed =
                !_usedForOrderBy ||
                (_usedForOrderBy && (_keywordsHelper.IsItOrRootOrParent(keywordOrType) || !_parsingConfig.RestrictOrderByToPropertyOrField));
            if (!keywordOrFunctionAllowed)
            {
                throw ParseError(Res.UnknownPropertyOrField, _textParser.CurrentToken.Text, TypeHelper.GetTypeName(_it?.Type));
            }

            switch (keywordOrType.CurrentType)
            {
                case AnyOfType.First:
                    switch (keywordOrType.First)
                    {
                        case KeywordsHelper.KEYWORD_IT:
                        case KeywordsHelper.SYMBOL_IT:
                            return ParseIt();

                        case KeywordsHelper.KEYWORD_PARENT:
                        case KeywordsHelper.SYMBOL_PARENT:
                            return ParseParent();

                        case KeywordsHelper.KEYWORD_ROOT:
                        case KeywordsHelper.SYMBOL_ROOT:
                            return ParseRoot();

                        case KeywordsHelper.FUNCTION_IIF:
                            return ParseFunctionIIF();

                        case KeywordsHelper.FUNCTION_ISNULL:
                            return ParseFunctionIsNull();

                        case KeywordsHelper.FUNCTION_NEW:
                            if (_parsingConfig.DisallowNewKeyword)
                            {
                                throw ParseError(Res.NewOperatorIsNotAllowed);
                            }
                            return ParseNew();

                        case KeywordsHelper.FUNCTION_NULLPROPAGATION:
                            return ParseFunctionNullPropagation();

                        case KeywordsHelper.FUNCTION_IS:
                            return ParseFunctionIs();

                        case KeywordsHelper.FUNCTION_AS:
                            return ParseFunctionAs();

                        case KeywordsHelper.FUNCTION_CAST:
                            return ParseFunctionCast();
                    }
                    break;

                case AnyOfType.Second:
                    _textParser.NextToken();
                    return keywordOrType.Second;

                case AnyOfType.Third:
                    return ParseTypeAccess(keywordOrType.Third, true);
            }
        }

        if (_symbols.TryGetValue(_textParser.CurrentToken.Text, out var value) ||
            _externals != null && _externals.TryGetValue(_textParser.CurrentToken.Text, out value) ||
            _internals.TryGetValue(_textParser.CurrentToken.Text, out value))
        {
            var expr = value as Expression;
            if (expr == null)
            {
                expr = Expression.Constant(value);
            }
            else
            {
                if (expr is LambdaExpression lambdaExpression)
                {
                    return ParseLambdaInvocation(lambdaExpression);
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

    private Expression ParseIt()
    {
        if (_it == null)
        {
            throw ParseError(Res.NoItInScope);
        }

        _textParser.NextToken();

        return _it;
    }

    private Expression ParseParent()
    {
        if (_parent == null)
        {
            throw ParseError(Res.NoParentInScope);
        }
        _textParser.NextToken();
        return _parent;
    }

    private Expression ParseRoot()
    {
        if (_root == null)
        {
            throw ParseError(Res.NoRootInScope);
        }
        _textParser.NextToken();
        return _root;
    }

    // isnull(a,b) function
    private Expression ParseFunctionIsNull()
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

    // iif(test, ifTrue, ifFalse) function
    private Expression ParseFunctionIIF()
    {
        int errorPos = _textParser.CurrentToken.Pos;
        _textParser.NextToken();

        Expression[] args = ParseArgumentList();
        if (args.Length != 3)
        {
            throw ParseError(errorPos, Res.IIFRequiresThreeArgs);
        }

        return GenerateConditional(args[0], args[1], args[2], false, errorPos);
    }

    // np(...) function
    private Expression ParseFunctionNullPropagation()
    {
        int errorPos = _textParser.CurrentToken.Pos;
        _textParser.NextToken();

        Expression[] args = ParseArgumentList();

        if (args.Length != 1 && args.Length != 2)
        {
            throw ParseError(errorPos, Res.NullPropagationRequiresCorrectArgs);
        }

        if (_expressionHelper.ExpressionQualifiesForNullPropagation(args[0]))
        {
            bool hasDefaultParameter = args.Length == 2;
            Expression expressionIfFalse = hasDefaultParameter ? args[1] : Expression.Constant(null);

            if (_expressionHelper.TryGenerateAndAlsoNotNullExpression(args[0], true, out Expression generatedExpression))
            {
                return GenerateConditional(generatedExpression, args[0], expressionIfFalse, true, errorPos);
            }

            return args[0];
        }

        throw ParseError(errorPos, Res.NullPropagationRequiresValidExpression);
    }

    // Is(...) function
    private Expression ParseFunctionIs()
    {
        int errorPos = _textParser.CurrentToken.Pos;
        string functionName = _textParser.CurrentToken.Text;
        _textParser.NextToken();

        Expression[] args = ParseArgumentList();

        if (args.Length != 1 && args.Length != 2)
        {
            throw ParseError(errorPos, Res.FunctionRequiresOneOrTwoArgs, functionName);
        }

        Expression typeArgument;
        Expression it;
        if (args.Length == 1)
        {
            typeArgument = args[0];
            it = _it!;
        }
        else
        {
            typeArgument = args[1];
            it = args[0];
        }

        return Expression.TypeIs(it, ResolveTypeFromArgumentExpression(functionName, typeArgument, args.Length));
    }

    // As(...) function
    private Expression ParseFunctionAs()
    {
        int errorPos = _textParser.CurrentToken.Pos;
        string functionName = _textParser.CurrentToken.Text;
        _textParser.NextToken();

        Expression[] args = ParseArgumentList();

        if (args.Length != 1 && args.Length != 2)
        {
            throw ParseError(errorPos, Res.FunctionRequiresOneOrTwoArgs, functionName);
        }

        Expression typeArgument;
        Expression it;
        if (args.Length == 1)
        {
            typeArgument = args[0];
            it = _it!;
        }
        else
        {
            typeArgument = args[1];
            it = args[0];
        }

        return Expression.TypeAs(it, ResolveTypeFromArgumentExpression(functionName, typeArgument, args.Length));
    }

    // Cast(...) function
    private Expression ParseFunctionCast()
    {
        int errorPos = _textParser.CurrentToken.Pos;
        string functionName = _textParser.CurrentToken.Text;
        _textParser.NextToken();

        Expression[] args = ParseArgumentList();

        if (args.Length != 1 && args.Length != 2)
        {
            throw ParseError(errorPos, Res.FunctionRequiresOneOrTwoArgs, functionName);
        }

        Expression typeArgument;
        Expression it;
        if (args.Length == 1)
        {
            typeArgument = args[0];
            it = _it!;
        }
        else
        {
            typeArgument = args[1];
            it = args[0];
        }

        var destinationType = ResolveTypeFromArgumentExpression(functionName, typeArgument, args.Length);

        if (TryGenerateConversion(it, destinationType, out var conversionExpression))
        {
            return conversionExpression;
        }

        return Expression.ConvertChecked(it, destinationType);
    }

    private Expression GenerateConditional(Expression test, Expression expressionIfTrue, Expression expressionIfFalse, bool nullPropagating, int errorPos)
    {
        if (test.Type != typeof(bool))
        {
            throw ParseError(errorPos, Res.FirstExprMustBeBool);
        }

        if (expressionIfTrue.Type != expressionIfFalse.Type)
        {
            // If expressionIfTrue is a null constant and expressionIfFalse is ValueType:
            if (Constants.IsNull(expressionIfTrue) && expressionIfFalse.Type.GetTypeInfo().IsValueType)
            {
                if (nullPropagating && _parsingConfig.NullPropagatingUseDefaultValueForNonNullableValueTypes)
                {
                    // If expressionIfFalse is a non-nullable type:
                    //  generate default expression from the expressionIfFalse-type for expressionIfTrue
                    // Else
                    //  create nullable constant from expressionIfTrue with type from expressionIfFalse

                    if (!TypeHelper.IsNullableType(expressionIfFalse.Type))
                    {
                        expressionIfTrue = _expressionHelper.GenerateDefaultExpression(expressionIfFalse.Type);
                    }
                    else
                    {
                        expressionIfTrue = Expression.Constant(null, expressionIfFalse.Type);
                    }
                }
                else
                {
                    // - create nullable constant from expressionIfTrue with type from expressionIfFalse
                    // - convert expressionIfFalse to nullable (unless it's already nullable)
                    var nullableType = TypeHelper.ToNullableType(expressionIfFalse.Type);
                    expressionIfTrue = Expression.Constant(null, nullableType);

                    if (!TypeHelper.IsNullableType(expressionIfFalse.Type))
                    {
                        expressionIfFalse = Expression.Convert(expressionIfFalse, nullableType);
                    }
                }

                return Expression.Condition(test, expressionIfTrue, expressionIfFalse);
            }

            // If expressionIfFalse is a null constant and expressionIfTrue is a ValueType:
            if (Constants.IsNull(expressionIfFalse) && expressionIfTrue.Type.GetTypeInfo().IsValueType)
            {
                if (nullPropagating && _parsingConfig.NullPropagatingUseDefaultValueForNonNullableValueTypes)
                {
                    // If expressionIfTrue is a non-nullable type:
                    //  generate default expression from the expressionIfFalse-type for expressionIfFalse
                    // Else
                    //  create nullable constant from expressionIfFalse with type from expressionIfTrue

                    if (!TypeHelper.IsNullableType(expressionIfTrue.Type))
                    {
                        expressionIfFalse = _expressionHelper.GenerateDefaultExpression(expressionIfTrue.Type);
                    }
                    else
                    {
                        expressionIfFalse = Expression.Constant(null, expressionIfTrue.Type);
                    }
                }
                else
                {
                    // - create nullable constant from expressionIfFalse with type from expressionIfTrue
                    // - convert expressionIfTrue to nullable (unless it's already nullable)

                    Type nullableType = TypeHelper.ToNullableType(expressionIfTrue.Type);
                    expressionIfFalse = Expression.Constant(null, nullableType);
                    if (!TypeHelper.IsNullableType(expressionIfTrue.Type))
                    {
                        expressionIfTrue = Expression.Convert(expressionIfTrue, nullableType);
                    }
                }

                return Expression.Condition(test, expressionIfTrue, expressionIfFalse);
            }

            var expr1As2 = !Constants.IsNull(expressionIfFalse) ? _parsingConfig.ExpressionPromoter.Promote(expressionIfTrue, expressionIfFalse.Type, true, false) : null;
            var expr2As1 = !Constants.IsNull(expressionIfTrue) ? _parsingConfig.ExpressionPromoter.Promote(expressionIfFalse, expressionIfTrue.Type, true, false) : null;
            if (expr1As2 != null && expr2As1 == null)
            {
                expressionIfTrue = expr1As2;
            }
            else if (expr2As1 != null && expr1As2 == null)
            {
                expressionIfFalse = expr2As1;
            }
            else
            {
                string type1 = !Constants.IsNull(expressionIfTrue) ? expressionIfTrue.Type.Name : "null";
                string type2 = !Constants.IsNull(expressionIfFalse) ? expressionIfFalse.Type.Name : "null";
                if (expr1As2 != null)
                {
                    throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                }

                throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
            }
        }

        return Expression.Condition(test, expressionIfTrue, expressionIfFalse);
    }

    // new (...) function
    private Expression ParseNew()
    {
        _textParser.NextToken();
        if (_textParser.CurrentToken.Id != TokenId.OpenParen &&
            _textParser.CurrentToken.Id != TokenId.OpenCurlyParen &&
            _textParser.CurrentToken.Id != TokenId.OpenBracket &&
            _textParser.CurrentToken.Id != TokenId.Identifier)
        {
            throw ParseError(Res.OpenParenOrIdentifierExpected);
        }

        Type? newType = null;
        if (_textParser.CurrentToken.Id == TokenId.Identifier)
        {
            var newTypeName = _textParser.CurrentToken.Text;

            _textParser.NextToken();

            while (_textParser.CurrentToken.Id is TokenId.Dot or TokenId.Plus)
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

            if (!_typeFinder.TryFindTypeByName(newTypeName, [_it, _parent, _root], false, out newType))
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
                string? propName;
                if (TokenIsIdentifier("as"))
                {
                    _textParser.NextToken();
                    propName = GetIdentifierAs();
                }
                else
                {
                    if (!TryGetMemberName(expr, out propName)) // TODO : investigate this
                    {
                        if (expr is MethodCallExpression methodCallExpression
                            && methodCallExpression.Arguments.Count == 1
                            && methodCallExpression.Arguments[0] is ConstantExpression methodCallExpressionArgument
                            && methodCallExpressionArgument.Type == typeof(string)
                            && properties.All(x => x.Name != (string?)methodCallExpressionArgument.Value))
                        {
                            propName = (string?)methodCallExpressionArgument.Value;
                        }
                        else
                        {
                            throw ParseError(exprPos, Res.MissingAsClause);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(propName))
                {
                    properties.Add(new DynamicProperty(propName!, expr.Type));
                }
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

    private Expression CreateArrayInitializerExpression(List<Expression> expressions, Type? newType)
    {
        if (expressions.Count == 0)
        {
            return Expression.NewArrayInit(newType ?? typeof(object));
        }

        if (newType != null)
        {
            var promotedExpressions = expressions
                .Select(expression => _parsingConfig.ExpressionPromoter.Promote(expression, newType, true, true))
                .OfType<Expression>();
            return Expression.NewArrayInit(newType, promotedExpressions);
        }

        return Expression.NewArrayInit(expressions.All(expression => expression.Type == expressions[0].Type) ? expressions[0].Type : typeof(object), expressions);
    }

    private Expression CreateNewExpression(List<DynamicProperty> properties, List<Expression> expressions, Type? newType)
    {
        // http://solutionizing.net/category/linq/
        Type? type = newType ?? _resultType;

        if (type == null)
        {
#if UAP10_0
            type = typeof(DynamicClass);
            Type typeForKeyValuePair = typeof(KeyValuePair<string, object>);

            ConstructorInfo constructorForKeyValuePair = typeForKeyValuePair.GetTypeInfo().DeclaredConstructors.First();

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

            // Get the "public DynamicClass(KeyValuePair<string, object>[])" constructor
            ConstructorInfo constructor = type.GetTypeInfo().DeclaredConstructors.First();

            return Expression.New(constructor, newArrayExpression);
#else
            type = DynamicClassFactory.CreateType(properties, _createParameterCtor);
#endif
        }

        // Option 1. Try to bind via properties (TODO : investigate if this code block is 100% correct and is needed) 
        var propertyInfos = type.GetProperties();
        if (type.GetTypeInfo().BaseType == typeof(DynamicClass))
        {
            propertyInfos = propertyInfos.Where(x => x.Name != "Item").ToArray();
        }

        var propertyTypes = propertyInfos.Select(p => p.PropertyType).ToArray();
        var ctor = type.GetConstructor(propertyTypes);
        if (ctor != null)
        {
            var constructorParameters = ctor.GetParameters();
            if (constructorParameters.Length == expressions.Count)
            {
                var bindParametersSequentially = !properties.All(p => constructorParameters
                    .Any(cp => cp.Name == p.Name && (cp.ParameterType == p.Type || p.Type == Nullable.GetUnderlyingType(cp.ParameterType))));
                var expressionsPromoted = new List<Expression>();

                // Loop all expressions and promote if needed
                for (int i = 0; i < constructorParameters.Length; i++)
                {
                    if (bindParametersSequentially)
                    {
                        expressionsPromoted.AddIfNotNull(_parsingConfig.ExpressionPromoter.Promote(expressions[i], propertyTypes[i], true, true));
                    }
                    else
                    {
                        var propertyType = constructorParameters[i].ParameterType;
                        var cParameterName = constructorParameters[i].Name;
                        var propertyAndIndex = properties.Select((p, index) => new { p, index })
                            .First(p => p.p.Name == cParameterName && (p.p.Type == propertyType || p.p.Type == Nullable.GetUnderlyingType(propertyType)));

                        // Promote from Type to Nullable Type if needed
                        expressionsPromoted.AddIfNotNull(_parsingConfig.ExpressionPromoter.Promote(expressions[propertyAndIndex.index], propertyType, true, true));
                    }
                }

                return Expression.New(ctor, expressionsPromoted, (IEnumerable<MemberInfo>)propertyInfos);
            }
        }

        // Option 2. Try to find a constructor with the exact argument-types and exact same order
        var constructorArgumentTypes = properties.Select(p => p.Type).ToArray();
        var exactConstructor = type.GetConstructor(constructorArgumentTypes);
        if (exactConstructor != null)
        {
            // Promote from Type to Nullable Type if needed
            var expressionsPromoted = exactConstructor.GetParameters()
                .Select((t, i) => _parsingConfig.ExpressionPromoter.Promote(expressions[i], t.ParameterType, true, true))
                .OfType<Expression>()
                .ToArray();

            return Expression.New(exactConstructor, expressionsPromoted);
        }

        // Option 2. Call the default (empty) constructor and set the members
        var memberBindings = new MemberBinding[properties.Count];
        for (var i = 0; i < memberBindings.Length; i++)
        {
            var propertyOrFieldName = properties[i].Name;
            Type propertyOrFieldType;
            MemberInfo memberInfo;
            var propertyInfo = type.GetProperty(propertyOrFieldName);
            if (propertyInfo != null)
            {
                memberInfo = propertyInfo;
                propertyOrFieldType = propertyInfo.PropertyType;
            }
            else
            {
                var fieldInfo = type.GetField(propertyOrFieldName);
                if (fieldInfo == null)
                {
                    throw ParseError(Res.UnknownPropertyOrField, propertyOrFieldName, TypeHelper.GetTypeName(type));
                }

                memberInfo = fieldInfo;
                propertyOrFieldType = fieldInfo.FieldType;
            }

            // Call Promote and if that returns false, just try to convert the expression to the destination type using Expression.Convert
            var promoted = _parsingConfig.ExpressionPromoter.Promote(expressions[i], propertyOrFieldType, true, true) ?? Expression.Convert(expressions[i], propertyOrFieldType);
            memberBindings[i] = Expression.Bind(memberInfo, promoted);
        }

        return Expression.MemberInit(Expression.New(type), memberBindings);
    }

    private Expression ParseLambdaInvocation(LambdaExpression lambda)
    {
        int errorPos = _textParser.CurrentToken.Pos;
        _textParser.NextToken();
        Expression[] args = ParseArgumentList();

        Expression? nullExpressionReference = null;
        if (_methodFinder.FindMethod(lambda.Type, nameof(Expression.Invoke), false, ref nullExpressionReference, ref args, out _) != 1)
        {
            throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
        }

        return Expression.Invoke(lambda, args);
    }

    private Expression ParseTypeAccess(Type type, bool getNext)
    {
        int errorPos = _textParser.CurrentToken.Pos;
        if (getNext)
        {
            _textParser.NextToken();
        }

        if (_textParser.CurrentToken.Id == TokenId.Question)
        {
            if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
            {
                throw ParseError(errorPos, Res.TypeHasNoNullableForm, TypeHelper.GetTypeName(type));
            }

            type = typeof(Nullable<>).MakeGenericType(type);
            _textParser.NextToken();
        }

        // This is a shorthand for explicitly converting a string to something
        var isShorthand = _textParser.CurrentToken.Id == TokenId.StringLiteral;
        if (_textParser.CurrentToken.Id == TokenId.OpenParen || isShorthand)
        {
            Expression[] args;
            if (isShorthand)
            {
                var expressionOrType = ParseStringLiteral(true);
                args = [expressionOrType.First];
            }
            else
            {
                args = ParseArgumentList();
            }

            // If only 1 argument and
            // - the arg is ConstantExpression, return the conversion
            // OR
            // - the arg is null, return the conversion (Can't use constructor)
            //
            // Then try to GenerateConversion

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (args.Length == 1 && (args[0] == null || args[0] is ConstantExpression) && TryGenerateConversion(args[0], type, out var generatedExpression))
            {
                return generatedExpression;
            }

            // If only 1 argument, and if the type is a ValueType and argType is also a ValueType, just Convert
            if (args.Length == 1)
            {
                Type argType = args[0].Type;

                if (type.GetTypeInfo().IsValueType && TypeHelper.IsNullableType(type) && argType.GetTypeInfo().IsValueType)
                {
                    return Expression.Convert(args[0], type);
                }
            }

            var constructorsWithOutPointerArguments = type.GetConstructors()
                .Where(c => !c.GetParameters().Any(p => p.ParameterType.GetTypeInfo().IsPointer))
                .ToArray();
            switch (_methodFinder.FindBestMethodBasedOnArguments(constructorsWithOutPointerArguments, ref args, out var method))
            {
                case 0:
                    if (args.Length == 1 && TryGenerateConversion(args[0], type, out generatedExpression))
                    {
                        return generatedExpression;
                    }

                    throw ParseError(errorPos, Res.NoMatchingConstructor, TypeHelper.GetTypeName(type));

                case 1:
                    return Expression.New((ConstructorInfo)method!, args);

                default:
                    throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, TypeHelper.GetTypeName(type));
            }
        }

        _textParser.ValidateToken(TokenId.Dot, Res.DotOrOpenParenOrStringLiteralExpected);
        _textParser.NextToken();

        return ParseMemberAccess(type, null);
    }

    private bool TryGenerateConversion(Expression sourceExpression, Type destinationType, [NotNullWhen(true)] out Expression? expression)
    {
        Type exprType = sourceExpression.Type;
        if (exprType == destinationType)
        {
            expression = sourceExpression;
            return true;
        }

        if (exprType.GetTypeInfo().IsValueType && destinationType.GetTypeInfo().IsValueType)
        {
            if ((TypeHelper.IsNullableType(exprType) || TypeHelper.IsNullableType(destinationType)) && TypeHelper.GetNonNullableType(exprType) == TypeHelper.GetNonNullableType(destinationType))
            {
                expression = Expression.Convert(sourceExpression, destinationType);
                return true;
            }

            if ((TypeHelper.IsNumericType(exprType) || TypeHelper.IsEnumType(exprType)) && TypeHelper.IsNumericType(destinationType) || TypeHelper.IsEnumType(destinationType))
            {
                expression = Expression.ConvertChecked(sourceExpression, destinationType);
                return true;
            }
        }

        if (exprType.IsAssignableFrom(destinationType) || destinationType.IsAssignableFrom(exprType) || exprType.GetTypeInfo().IsInterface || destinationType.GetTypeInfo().IsInterface)
        {
            expression = Expression.Convert(sourceExpression, destinationType);
            return true;
        }

        // Try to Parse the string rather than just generate the convert statement
        if (sourceExpression is ConstantExpression { Value: string constantStringValue })
        {
            var typeConvertor = _typeConverterFactory.GetConverter(destinationType);
            if (typeConvertor.CanConvertFrom(typeof(string)))
            {
                var value = typeConvertor.ConvertFromInvariantString(constantStringValue);
                expression = Expression.Constant(value, destinationType);
                return true;
            }
        }

        // Check if there are any explicit conversion operators on the source type which fit the requirement (cast to the return type).
        bool explicitOperatorAvailable = exprType.GetTypeInfo().GetDeclaredMethods("op_Explicit").Any(m => m.ReturnType == destinationType);
        if (explicitOperatorAvailable)
        {
            expression = Expression.Convert(sourceExpression, destinationType);
            return true;
        }

        // Try to find a destinationType.Parse(...) method for the specific sourceExpression Type.
        var parseMethod = destinationType.GetMethod("Parse", new Type[] { sourceExpression.Type });
        if (parseMethod != null)
        {
            expression = Expression.Call(parseMethod, sourceExpression);
            return true;
        }

        expression = null;
        return false;
    }

    private Expression ParseMemberAccess(Type? type, Expression? expression, string? id = null)
    {
        var errorPos = _textParser.CurrentToken.Pos;

        // In case the expression is not null and the type is null, get the type from the expression.
        if (expression != null)
        {
            type ??= expression.Type;
        }

        // In case the id is not defined, get it and move the TextParser forward.
        if (id == null)
        {
            id = GetIdentifier();
            _textParser.NextToken();
        }

        // Parse as Lambda
        if (_textParser.CurrentToken.Id == TokenId.Lambda && _it?.Type == type)
        {
            return ParseAsLambda(id);
        }

        if (_textParser.CurrentToken.Id == TokenId.OpenParen)
        {
            Expression[]? args = null;

            var isStaticAccess = expression == null;
            var isConstantString = expression is ConstantExpression { Value: string };
            var isStringWithStringMethod = type == typeof(string) && _methodFinder.ContainsMethod(type, id, isStaticAccess);
            var isApplicableForEnumerable = !isStaticAccess && !isConstantString && !isStringWithStringMethod;

            if (isApplicableForEnumerable &&
                TypeHelper.TryFindGenericType(typeof(IEnumerable<>), type, out var enumerableType) &&
                TryParseEnumerable(expression!, enumerableType, id, type, out args, out var enumerableExpression))
            {
                return enumerableExpression;
            }

            // If args is not set by TryParseEnumerable (in case when the expression is not an Enumerable), do parse the argument list here.
            args ??= ParseArgumentList();
            switch (_methodFinder.FindMethod(type, id, isStaticAccess, ref expression, ref args, out var methodBase))
            {
                case 0:
                    throw ParseError(errorPos, Res.NoApplicableMethod, id, TypeHelper.GetTypeName(type));

                case 1:
                    var method = (MethodInfo)methodBase!;
                    if (!PredefinedTypesHelper.IsPredefinedType(_parsingConfig, method.DeclaringType!) && !PredefinedMethodsHelper.IsPredefinedMethod(_parsingConfig, method))
                    {
                        throw ParseError(errorPos, Res.MethodIsInaccessible, id, TypeHelper.GetTypeName(method.DeclaringType!));
                    }

                    MethodInfo methodToCall;
                    if (!method.IsGenericMethod)
                    {
                        methodToCall = method;
                    }
                    else
                    {
                        var genericParameters = method.GetParameters().Where(p => p.ParameterType.IsGenericParameter);
                        var typeArguments = genericParameters.Select(a => args[a.Position].Type);
                        methodToCall = method.MakeGenericMethod(typeArguments.ToArray());
                    }

                    return CallMethod(expression, methodToCall, args);

                default:
                    throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, TypeHelper.GetTypeName(type));
            }
        }

        if (TypeHelper.TryParseEnum(id, type, out var enumValue))
        {
            return Expression.Constant(enumValue);
        }

#if UAP10_0 || NETSTANDARD1_3
        if (type == typeof(DynamicClass))
        {
            return Expression.MakeIndex(expression, typeof(DynamicClass).GetProperty("Item"), new[] { Expression.Constant(id) });
        }
#endif
        if (TryFindPropertyOrField(type!, id, expression, out var propertyOrFieldExpression))
        {
            return propertyOrFieldExpression;
        }

        // #357 #662
        var extraCheck = !_parsingConfig.PrioritizePropertyOrFieldOverTheType ||
                         _parsingConfig.PrioritizePropertyOrFieldOverTheType && expression != null;

        if (!_parsingConfig.DisableMemberAccessToIndexAccessorFallback && extraCheck)
        {
            var indexerMethod = expression?.Type.GetMethod("get_Item", new[] { typeof(string) });
            if (indexerMethod != null)
            {
                return Expression.Call(expression, indexerMethod, Expression.Constant(id));
            }
        }

#if !NET35 && !UAP10_0 && !NETSTANDARD1_3
        if (type == typeof(object))
        {
            // The member is a dynamic or ExpandoObject, so convert this
            return _expressionHelper.ConvertToExpandoObjectAndCreateDynamicExpression(expression!, type, id);
        }
#endif

        // This could be enum like "A.B.C.MyEnum.Value1" or "A.B.C+MyEnum.Value1".
        //
        // Or it's a nested (static) class with a
        // - static property like "NestedClass.MyProperty"
        // - static method like "NestedClass.MyMethod"
        if (_textParser.CurrentToken.Id is TokenId.Dot or TokenId.Plus)
        {
            return ParseAsEnumOrNestedClass(id);
        }

        throw ParseError(errorPos, Res.UnknownPropertyOrField, id, TypeHelper.GetTypeName(type));
    }

    private bool TryFindPropertyOrField(Type type, string id, Expression? expression, [NotNullWhen(true)] out Expression? propertyOrFieldExpression)
    {
        var member = FindPropertyOrField(type, id, expression == null);
        switch (member)
        {
            case PropertyInfo property:
                var propertyIsStatic = property?.GetGetMethod().IsStatic ?? property?.GetSetMethod().IsStatic ?? false;
                propertyOrFieldExpression = propertyIsStatic ? Expression.Property(null, property) : Expression.Property(expression, property);
                return true;

            case FieldInfo field:
                propertyOrFieldExpression = field.IsStatic ? Expression.Field(null, field) : Expression.Field(expression, field);
                return true;

            default:
                propertyOrFieldExpression = null;
                return false;
        }
    }

    private static Expression CallMethod(Expression? expression, MethodInfo methodToCall, Expression[] args)
    {
#if NET35
        return Expression.Call(expression, methodToCall, args);
#else
        if (!args.OfType<ParameterExpression>().Any(p => p.IsByRef))
        {
            return Expression.Call(expression, methodToCall, args);
        }

        // A list which is used to store all method arguments.
        var newList = new List<Expression>();

        // A list which contains the variable expression for the 'out' parameter, and also contains the returnValue variable.
        var blockList = new List<ParameterExpression>();

        foreach (var arg in args)
        {
            if (arg is ParameterExpression { IsByRef: true } parameterExpression)
            {
                // Create a variable expression to hold the 'out' parameter.
                var variable = Expression.Variable(parameterExpression.Type, parameterExpression.Name);

                newList.Add(variable);
                blockList.Add(variable);
            }
            else
            {
                newList.Add(arg);
            }
        }

        // Create a method call expression to call the method
        var methodCall = Expression.Call(expression, methodToCall, newList);

        // Create a variable to hold the return value
        var returnValue = Expression.Variable(methodToCall.ReturnType);

        // Add this return variable to the blockList
        blockList.Add(returnValue);

        // Create the block to return the boolean value.
        var block = Expression.Block(
            blockList.ToArray(),
            Expression.Assign(returnValue, methodCall),
            returnValue
        );

        // Create the lambda expression (note that expression must be a ParameterExpression).
        return Expression.Lambda(block, (ParameterExpression)expression!);
#endif
    }

    private Expression ParseAsLambda(string id)
    {
        // This might be an internal variable for use within a lambda expression, so store it as such
        _internals.Add(id, _it!);
        string previousItName = ItName;

        // Also store ItName (only once)
        if (string.Equals(ItName, KeywordsHelper.KEYWORD_IT))
        {
            ItName = id;
        }

        // next
        _textParser.NextToken();

        LastLambdaItName = ItName;
        var exp = ParseConditionalOperator();

        // Restore previous context and clear internals
        _internals.Remove(id);
        ItName = previousItName;

        return exp;
    }

    private Expression ParseAsEnumOrNestedClass(string id)
    {
        var parts = new List<string> { id };

        while (_textParser.CurrentToken.Id is TokenId.Dot or TokenId.Plus)
        {
            if (_textParser.CurrentToken.Id is TokenId.Dot or TokenId.Plus)
            {
                parts.Add(_textParser.CurrentToken.Text);
                _textParser.NextToken();
            }

            if (_textParser.CurrentToken.Id == TokenId.Identifier)
            {
                parts.Add(_textParser.CurrentToken.Text);
                _textParser.NextToken();
            }
        }

        var typeAsString = string.Concat(parts.Take(parts.Count - 2).ToArray());
        if (!_typeFinder.TryFindTypeByName(typeAsString, null, true, out var type))
        {
            throw ParseError(_textParser.CurrentToken.Pos, Res.TypeNotFound, typeAsString);
        }

        var isEnum = TypeHelper.IsEnumType(type);

        var identifier = parts.LastOrDefault();
        if (identifier == null)
        {
            if (isEnum)
            {
                throw ParseError(_textParser.CurrentToken.Pos, Res.EnumTypeNotFound, typeAsString);
            }

            throw ParseError(_textParser.CurrentToken.Pos, Res.UnknownIdentifier, typeAsString);
        }

        if (isEnum)
        {
            if (TypeHelper.TryParseEnum(identifier, type, out var enumValue))
            {
                return Expression.Constant(enumValue);
            }

            throw ParseError(_textParser.CurrentToken.Pos, Res.EnumValueNotDefined, identifier, typeAsString);
        }

        return ParseMemberAccess(type, null, identifier);
    }

    private bool TryParseEnumerable(Expression instance, Type enumerableType, string methodName, Type? type, out Expression[]? args, [NotNullWhen(true)] out Expression? expression)
    {
        var elementType = enumerableType.GetTypeInfo().GetGenericTypeArguments()[0];

        // Keep the current _parent.
        var oldParent = _parent;

        // Set the _parent to the current _it.
        _parent = _it;

        // Set the outerIt to the current _it.
        var outerIt = _it;

        // Create a new innerIt based on the elementType.
        var innerIt = ParameterExpressionHelper.CreateParameterExpression(elementType, string.Empty, _parsingConfig.RenameEmptyParameterExpressionNames);

        if (new[] { "Contains", "ContainsKey", "Skip", "Take" }.Contains(methodName))
        {
            // For any method that acts on the parent element type, we need to specify the outerIt as scope.
            _it = outerIt;
        }
        else
        {
            // Else we need to specify the innerIt as scope.
            _it = innerIt;
        }

        args = ParseArgumentList();
        var copyArgs = args.ToArray();

        // Revert the _it and _parent to the old values.
        _it = outerIt;
        _parent = oldParent;

        var theType = type ?? instance.Type;
        if (theType == typeof(string) && _methodFinder.ContainsMethod(theType, methodName, false, instance, ref args))
        {
            // In case the type is a string, and does contain the methodName (like "IndexOf") then:
            // - restore the args
            // - set the expression to null
            // - return false to indicate that the methodName is not an Enumerable method
            args = copyArgs;
            expression = null;
            return false;
        }

        if (type != null && TypeHelper.IsDictionary(type) && _methodFinder.ContainsMethod(type, methodName, false))
        {
            var dictionaryMethod = type.GetMethod(methodName)!;
            expression = Expression.Call(instance, dictionaryMethod, args);
            return true;
        }

        // #794 - Check if the method is an aggregate (Average or Sum) method and try to update the arguments to match the method arguments
        var isAggregateMethod = _methodFinder.CheckAggregateMethodAndTryUpdateArgsToMatchMethodArgs(methodName, ref args);

        var callType = typeof(Enumerable);
        if (TypeHelper.TryFindGenericType(typeof(IQueryable<>), type, out _) && _methodFinder.ContainsMethod(typeof(Queryable), methodName))
        {
            callType = typeof(Queryable);
        }

        // #633 / #856
        // For Average/Sum without any arguments, try to find the non-generic Average/Sum method on the callType for the supplied parameter type.
        if (isAggregateMethod && args.Length == 0 && _methodFinder.TryFindAggregateMethod(callType, methodName, theType, out var aggregateMethod))
        {
            expression = Expression.Call(null, aggregateMethod, instance);
            return true;
        }

        Type[] typeArgs;
        if (new[] { "OfType", "Cast" }.Contains(methodName))
        {
            if (args.Length != 1)
            {
                throw ParseError(_textParser.CurrentToken.Pos, Res.FunctionRequiresOneArg, methodName);
            }

            typeArgs = [ResolveTypeFromArgumentExpression(methodName, args[0])];
            args = [];
        }
        else if (new[] { "Max", "Min", "Select", "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending", "GroupBy" }.Contains(methodName))
        {
            if (args.Length == 2)
            {
                typeArgs = [elementType, args[0].Type, args[1].Type];
            }
            else if (args.Length == 1)
            {
                typeArgs = [elementType, args[0].Type];
            }
            else
            {
                typeArgs = [elementType];
            }
        }
        else if (methodName == "SelectMany")
        {
            var bodyType = Expression.Lambda(args[0], innerIt).Body.Type;
            var interfaces = bodyType.GetInterfaces().Union([bodyType]);
            var interfaceType = interfaces.Single(i => i.Name == typeof(IEnumerable<>).Name);
            var resultType = interfaceType.GetTypeInfo().GetGenericTypeArguments()[0];
            typeArgs = [elementType, resultType];
        }
        else
        {
            typeArgs = [elementType];
        }

        if (args.Length == 0)
        {
            args = [instance];
        }
        else
        {
            if (new[] { "Concat", "Contains", "ContainsKey", "DefaultIfEmpty", "Except", "Intersect", "Skip", "Take", "Union", "SequenceEqual" }.Contains(methodName))
            {
                args = [instance, args[0]];
            }
            else
            {
                if (args.Length == 2)
                {
                    args = [instance, Expression.Lambda(args[0], innerIt), Expression.Lambda(args[1], innerIt)];
                }
                else
                {
                    args = [instance, Expression.Lambda(args[0], innerIt)];
                }
            }
        }

        expression = Expression.Call(callType, methodName, typeArgs, args);
        return true;
    }

    private Type ResolveTypeFromArgumentExpression(string functionName, Expression argumentExpression, int? arguments = null)
    {
        var argument = arguments == null ? string.Empty : arguments == 1 ? "first " : "second ";

        switch (argumentExpression)
        {
            case ConstantExpression constantExpression:
                return ResolveTypeFromExpressionValue(functionName, constantExpression, argument);

            case MemberExpression memberExpression:
                if (_expressionHelper.TryUnwrapAsConstantExpression(memberExpression, out var unwrappedConstantExpression))
                {
                    return ResolveTypeFromExpressionValue(functionName, unwrappedConstantExpression, argument);
                }

                break;
        }

        throw ParseError(_textParser.CurrentToken.Pos, Res.FunctionRequiresNotNullArgOfType, functionName, argument, "ConstantExpression");
    }

    private Type ResolveTypeFromExpressionValue(string functionName, ConstantExpression constantExpression, string argument)
    {
        switch (constantExpression.Value)
        {
            case string typeName:
                return ResolveTypeStringFromArgument(typeName);

            case Type type:
                return type;

            default:
                throw ParseError(_textParser.CurrentToken.Pos, Res.FunctionRequiresNotNullArgOfType, functionName, argument, "string or System.Type");
        }
    }

    private Type ResolveTypeStringFromArgument(string typeName)
    {
        var typeIsNullable = false;

        if (typeName.EndsWith("?"))
        {
            typeName = typeName.TrimEnd('?');
            typeIsNullable = true;
        }

        if (!_typeFinder.TryFindTypeByName(typeName, [_it, _parent, _root], true, out var type))
        {
            throw ParseError(_textParser.CurrentToken.Pos, Res.TypeNotFound, typeName);
        }

        return typeIsNullable ? TypeHelper.ToNullableType(type) : type;
    }

    private Expression[] ParseArgumentList()
    {
        _textParser.ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
        _textParser.NextToken();

        var args = _textParser.CurrentToken.Id != TokenId.CloseParen ? ParseArguments() : [];

        _textParser.ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
        _textParser.NextToken();

        return args;
    }

    private Expression[] ParseArguments()
    {
        var argList = new List<Expression>();

        while (true)
        {
            var argumentExpression = ParseOutKeyword();

            _expressionHelper.WrapConstantExpression(ref argumentExpression);

            argList.Add(argumentExpression);

            if (_textParser.CurrentToken.Id != TokenId.Comma)
            {
                break;
            }

            _textParser.NextToken();
        }

        return argList.ToArray();
    }

    private Expression ParseElementAccess(Expression expr)
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

            var indexExpression = _parsingConfig.ExpressionPromoter.Promote(args[0], typeof(int), true, false);
            if (indexExpression == null)
            {
                throw ParseError(errorPos, Res.InvalidIndex);
            }

            return Expression.ArrayIndex(expr, indexExpression);
        }

        switch (_methodFinder.FindIndexer(expr.Type, args, out var mb))
        {
            case 0:
                throw ParseError(errorPos, Res.NoApplicableIndexer,
                    TypeHelper.GetTypeName(expr.Type));

            case 1:
                var indexMethod = (MethodInfo)mb!;
                var indexParameterType = indexMethod.GetParameters().First().ParameterType;

                var indexArgumentExpression = args[0]; // Indexer only has 1 parameter, so we can use args[0] here
                if (indexParameterType != indexArgumentExpression.Type)
                {
                    indexArgumentExpression = Expression.Convert(indexArgumentExpression, indexParameterType);
                }

                return Expression.Call(expr, indexMethod, indexArgumentExpression);

            default:
                throw ParseError(errorPos, Res.AmbiguousIndexerInvocation, TypeHelper.GetTypeName(expr.Type));
        }
    }

    internal static Type ToNullableType(Type type)
    {
        Check.NotNull(type, nameof(type));

        if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
        {
            throw ParseError(-1, Res.TypeHasNoNullableForm, TypeHelper.GetTypeName(type));
        }

        return typeof(Nullable<>).MakeGenericType(type);
    }

    private static bool TryGetMemberName(Expression expression, out string? memberName)
    {
        var memberExpression = expression as MemberExpression;
        if (memberExpression == null && expression.NodeType == ExpressionType.Coalesce)
        {
            memberExpression = (expression as BinaryExpression)?.Left as MemberExpression;
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

    private void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
    {
        Expression[] args = { expr };

        if (!_methodFinder.ContainsMethod(signatures, "F", false, null, ref args))
        {
            throw IncompatibleOperandError(opName, expr, errorPos);
        }

        expr = args[0];
    }

    private bool TryGetOverloadedEqualityOperator(TokenId tokenId, ref Expression left, ref Expression right, ref Expression[] args)
    {
        if (tokenId is TokenId.DoubleEqual or TokenId.Equal)
        {
            const string method = "op_Equality";
            return _methodFinder.ContainsMethod(left.Type, method, true, null, ref args) ||
                   _methodFinder.ContainsMethod(right.Type, method, true, null, ref args);
        }

        if (tokenId is TokenId.ExclamationEqual or TokenId.LessGreater)
        {
            const string method = "op_Inequality";
            return _methodFinder.ContainsMethod(left.Type, method, true, null, ref args) ||
                   _methodFinder.ContainsMethod(right.Type, method, true, null, ref args);
        }

        return false;
    }

    private bool TryGetOverloadedImplicitOperator(TokenId tokenId, ref Expression left, ref Expression right)
    {
        const string methodName = "op_Implicit";
        if (tokenId is not (TokenId.Ampersand or TokenId.DoubleAmpersand or TokenId.Bar or TokenId.DoubleBar))
        {
            return false;
        }

        Expression? expression = null;
        var overloadedImplicitOperatorFound = false;

        // If the left is not a boolean, try to find the "op_Implicit" method on the left which takes the left as parameter and returns a boolean.
        if (left.Type != typeof(bool))
        {
            var args = new[] { left };
            if (_methodFinder.FindMethod(left.Type, methodName, true, ref expression, ref args, out var methodBase) > 0 && methodBase is MethodInfo methodInfo && methodInfo.ReturnType == typeof(bool))
            {
                left = Expression.Call(methodInfo, left);
                overloadedImplicitOperatorFound = true;
            }
        }

        // If the right is not a boolean, try to find the "op_Implicit" method on the right which takes the right as parameter and returns a boolean.
        if (right.Type != typeof(bool))
        {
            var args = new[] { right };
            if (_methodFinder.FindMethod(right.Type, methodName, true, ref expression, ref args, out var methodBase) > 0 && methodBase is MethodInfo methodInfo && methodInfo.ReturnType == typeof(bool))
            {
                right = Expression.Call(methodInfo, right);
                overloadedImplicitOperatorFound = true;
            }
        }

        return overloadedImplicitOperatorFound;
    }

    private void CheckAndPromoteOperands(Type signatures, TokenId opId, string opName, ref Expression left, ref Expression right, int errorPos)
    {
        Expression[] args = { left, right };

        // 1. Try to find the Equality/Inequality operator
        // 2. Try to find the method with the given signature
        if (TryGetOverloadedEqualityOperator(opId, ref left, ref right, ref args) || _methodFinder.ContainsMethod(signatures, "F", false, null, ref args))
        {
            left = args[0];
            right = args[1];

            return;
        }

        // 3. Try to find the Implicit operator
        if (TryGetOverloadedImplicitOperator(opId, ref left, ref right))
        {
            return;
        }

        throw IncompatibleOperandsError(opName, left, right, errorPos);
    }

    private static Exception IncompatibleOperandError(string opName, Expression expr, int errorPos)
    {
        return ParseError(errorPos, Res.IncompatibleOperand, opName, TypeHelper.GetTypeName(expr.Type));
    }

    private static Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int errorPos)
    {
        return ParseError(errorPos, Res.IncompatibleOperands, opName, TypeHelper.GetTypeName(left.Type), TypeHelper.GetTypeName(right.Type));
    }

    private MemberInfo? FindPropertyOrField(Type type, string memberName, bool staticAccess)
    {
#if !(UAP10_0 || NETSTANDARD)
        var extraBindingFlag = _parsingConfig.PrioritizePropertyOrFieldOverTheType && staticAccess ? BindingFlags.Static : BindingFlags.Instance | BindingFlags.Static;
        var bindingFlags = BindingFlags.Public | BindingFlags.DeclaredOnly | extraBindingFlag;
        foreach (Type t in TypeHelper.GetSelfAndBaseTypes(type))
        {
            var findMembersType = _parsingConfig?.IsCaseSensitive == true ? Type.FilterName : Type.FilterNameIgnoreCase;
            var members = t.FindMembers(MemberTypes.Property | MemberTypes.Field, bindingFlags, findMembersType, memberName);

            if (members.Length != 0)
            {
                return members[0];
            }
        }
        return null;
#else
        var isCaseSensitive = _parsingConfig.IsCaseSensitive == true;
        foreach (Type t in TypeHelper.GetSelfAndBaseTypes(type))
        {
            // Try to find a property with the specified memberName
            MemberInfo? member = t.GetTypeInfo().DeclaredProperties
                .FirstOrDefault(x => (!staticAccess || x.GetAccessors(true)[0].IsStatic) && (x.Name == memberName || (!isCaseSensitive && x.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase))));
            if (member != null)
            {
                return member;
            }

            // If no property is found, try to get a field with the specified memberName
            member = t.GetTypeInfo().DeclaredFields.FirstOrDefault(x => (!staticAccess || x.IsStatic) && ((x.Name == memberName) || (!isCaseSensitive && x.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase))));
            if (member != null)
            {
                return member;
            }

            // No property or field is found, try the base type.
        }
        return null;
#endif
    }

    private bool TokenIsIdentifier(string id)
    {
        return _textParser.TokenIsIdentifier(id);
    }
    
    private string GetIdentifier()
    {
        _textParser.ValidateToken(TokenId.Identifier, Res.IdentifierExpected);

        return SanitizeId(_textParser.CurrentToken.Text);
    }

    private string GetIdentifierAs()
    {
        _textParser.ValidateToken(TokenId.Identifier, Res.IdentifierExpected);

        if (!_parsingConfig.SupportDotInPropertyNames)
        {
            var id = SanitizeId(_textParser.CurrentToken.Text);
            _textParser.NextToken();
            return id;
        }

        var parts = new List<string>();
        while (_textParser.CurrentToken.Id is TokenId.Dot or TokenId.Identifier)
        {
            parts.Add(_textParser.CurrentToken.Text);
            _textParser.NextToken();
        }

        return SanitizeId(string.Concat(parts));
    }

    private static string SanitizeId(string id)
    {
        if (id.Length > 1 && id[0] == '@')
        {
            id = id.Substring(1);
        }

        return id;
    }

    private Exception ParseError(string format, params object[] args)
    {
        return ParseError(_textParser.CurrentToken.Pos, format, args);
    }

    private static Exception ParseError(int pos, string format, params object[] args)
    {
        return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
    }
}