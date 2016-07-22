using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;

namespace System.Linq.Dynamic.Core
{
    internal class ExpressionParser
    {
        struct Token
        {
            public TokenId id;
            public string text;
            public int pos;
        }

        enum TokenId
        {
            Unknown,
            End,
            Identifier,
            StringLiteral,
            IntegerLiteral,
            RealLiteral,
            Exclamation,
            Percent,
            Amphersand,
            OpenParen,
            CloseParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Dot,
            Slash,
            Colon,
            LessThan,
            Equal,
            GreaterThan,
            Question,
            OpenBracket,
            CloseBracket,
            Bar,
            ExclamationEqual,
            DoubleAmphersand,
            LessThanEqual,
            LessGreater,
            DoubleEqual,
            GreaterThanEqual,
            DoubleBar,
            DoubleGreaterThan,
            DoubleLessThan,
            NullCoalescing
        }

        interface ILogicalSignatures
        {
            void F(bool x, bool y);
            void F(bool? x, bool? y);
        }

        interface IArithmeticSignatures
        {
            void F(int x, int y);
            void F(uint x, uint y);
            void F(long x, long y);
            void F(ulong x, ulong y);
            void F(float x, float y);
            void F(double x, double y);
            void F(decimal x, decimal y);
            void F(int? x, int? y);
            void F(uint? x, uint? y);
            void F(long? x, long? y);
            void F(ulong? x, ulong? y);
            void F(float? x, float? y);
            void F(double? x, double? y);
            void F(decimal? x, decimal? y);
        }

        interface IRelationalSignatures : IArithmeticSignatures
        {
            void F(string x, string y);
            void F(char x, char y);
            void F(DateTime x, DateTime y);
            void F(DateTimeOffset x, DateTimeOffset y);
            void F(TimeSpan x, TimeSpan y);
            void F(char? x, char? y);
            void F(DateTime? x, DateTime? y);
            void F(DateTimeOffset? x, DateTimeOffset? y);
            void F(TimeSpan? x, TimeSpan? y);
        }

        interface IEqualitySignatures : IRelationalSignatures
        {
            void F(bool x, bool y);
            void F(bool? x, bool? y);

            // Disabled 4 lines below because of : https://github.com/StefH/System.Linq.Dynamic.Core/issues/19
            //void F(DateTime x, string y);
            //void F(DateTime? x, string y);
            //void F(string x, DateTime y);
            //void F(string x, DateTime? y);

            void F(Guid x, Guid y);
            void F(Guid? x, Guid? y);
            void F(Guid x, string y);
            void F(Guid? x, string y);
            void F(string x, Guid y);
            void F(string x, Guid? y);
        }

        interface IAddSignatures : IArithmeticSignatures
        {
            void F(DateTime x, TimeSpan y);
            void F(TimeSpan x, TimeSpan y);
            void F(DateTime? x, TimeSpan? y);
            void F(TimeSpan? x, TimeSpan? y);
        }

        interface ISubtractSignatures : IAddSignatures
        {
            void F(DateTime x, DateTime y);
            void F(DateTime? x, DateTime? y);
        }

        interface INegationSignatures
        {
            void F(int x);
            void F(long x);
            void F(float x);
            void F(double x);
            void F(decimal x);
            void F(int? x);
            void F(long? x);
            void F(float? x);
            void F(double? x);
            void F(decimal? x);
        }

        interface INotSignatures
        {
            void F(bool x);
            void F(bool? x);
        }

        interface IEnumerableSignatures
        {
            void Where(bool predicate);
            void Any();
            void Any(bool predicate);
            void First(bool predicate);
            void FirstOrDefault(bool predicate);
            void Single(bool predicate);
            void SingleOrDefault(bool predicate);
            void Last(bool predicate);
            void LastOrDefault(bool predicate);
            void All(bool predicate);
            void Count();
            void Count(bool predicate);
            void Min(object selector);
            void Max(object selector);
            void Sum(int selector);
            void Sum(int? selector);
            void Sum(long selector);
            void Sum(long? selector);
            void Sum(float selector);
            void Sum(float? selector);
            void Sum(double selector);
            void Sum(double? selector);
            void Sum(decimal selector);
            void Sum(decimal? selector);
            void Average(int selector);
            void Average(int? selector);
            void Average(long selector);
            void Average(long? selector);
            void Average(float selector);
            void Average(float? selector);
            void Average(double selector);
            void Average(double? selector);
            void Average(decimal selector);
            void Average(decimal? selector);
            void Select(object selector);
            void SelectMany(object selector);
            void OrderBy(object selector);
            void OrderByDescending(object selector);
            void Contains(object selector);
            void Skip(int count);
            void SkipWhile(bool predicate);
            void Take(int count);
            void TakeWhile(bool predicate);
            void Distinct();

            //Executors
            void Single();
            void SingleOrDefault();
            void First();
            void FirstOrDefault();
            void Last();
            void LastOrDefault();
        }

        // These shorthands have different name than actual type and therefore not recognized by default from the _predefinedTypes
        //
        static readonly Dictionary<string, Type> _predefinedTypesShorthands = new Dictionary<string, Type>
        {
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "short", typeof(short) },
            { "ushort", typeof(ushort) },
            { "long", typeof(long) },
            { "ulong", typeof(ulong) },
            { "bool", typeof(bool) },
            { "float", typeof(float) },
        };

        static readonly Dictionary<Type, int> _predefinedTypes = new Dictionary<Type, int> {
            { typeof(object), 0 },
            { typeof(bool), 0 },
            { typeof(char), 0 },
            { typeof(string), 0 },
            { typeof(sbyte), 0 },
            { typeof(byte), 0 },
            { typeof(short), 0 },
            { typeof(ushort), 0 },
            { typeof(int), 0 },
            { typeof(uint), 0 },
            { typeof(long), 0 },
            { typeof(ulong), 0 },
            { typeof(float), 0 },
            { typeof(double), 0 },
            { typeof(decimal), 0 },
            { typeof(DateTime), 0 },
            { typeof(DateTimeOffset), 0 },
            { typeof(TimeSpan), 0 },
            { typeof(Guid), 0 },
            { typeof(Math), 0 },
            { typeof(Convert), 0 },
            { typeof(Uri), 0 }
        };

        // These aliases are supposed to simply the where clause and make it more human readable
        // As an addition it is compatible with the OData.Filter specification
        //
        static readonly Dictionary<string, TokenId> _predefinedAliases = new Dictionary<string, TokenId>()
        {
            { "eq", TokenId.Equal },
            { "ne", TokenId.ExclamationEqual },
            { "neq", TokenId.ExclamationEqual },
            { "lt", TokenId.LessThan },
            { "le", TokenId.LessThanEqual },
            { "gt", TokenId.GreaterThan },
            { "ge", TokenId.GreaterThanEqual },
            { "and", TokenId.DoubleAmphersand },
            { "or", TokenId.DoubleBar },
            { "not", TokenId.Exclamation },
            { "mod", TokenId.Percent }
        };

        static readonly Expression TrueLiteral = Expression.Constant(true);
        static readonly Expression FalseLiteral = Expression.Constant(false);
        static readonly Expression NullLiteral = Expression.Constant(null);

        const string SYMBOL_IT = "$";
        const string SYMBOL_PARENT = "^";
        const string SYMBOL_ROOT = "~";

        const string KEYWORD_IT = "it";
        const string KEYWORD_PARENT = "parent";
        const string KEYWORD_ROOT = "root";
        const string KEYWORD_IIF = "iif";
        const string KEYWORD_NEW = "new";
        const string KEYWORD_ISNULL = "isnull";

        static readonly string methodOrderBy = nameof(Queryable.OrderBy);
        static readonly string methodOrderByDescending = nameof(Queryable.OrderByDescending);
        static readonly string methodThenBy = nameof(Queryable.ThenBy);
        static readonly string methodThenByDescending = nameof(Queryable.ThenByDescending);

        static Dictionary<string, object> _keywords;

        readonly Dictionary<string, object> _symbols;
        IDictionary<string, object> _externals;
        readonly Dictionary<Expression, string> _literals;
        ParameterExpression _it;
        ParameterExpression _parent;
        ParameterExpression _root;

        readonly string _text;
        int _textPos;
        readonly int _textLen;
        char _ch;
        Token _token;
        private static char NumberDecimalSeparator = '.';

        static ExpressionParser()
        {
#if !(NET35 || SILVERLIGHT || NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            //System.Data.Entity is always here, so overwrite short name of it with EntityFramework if EntityFramework is found.
            //EF5(or 4.x??), System.Data.Objects.DataClasses.EdmFunctionAttribute
            //There is also an System.Data.Entity, Version=3.5.0.0, but no Functions.
            UpdatePredefinedTypes("System.Data.Objects.EntityFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            UpdatePredefinedTypes("System.Data.Objects.SqlClient.SqlFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);
            UpdatePredefinedTypes("System.Data.Objects.SqlClient.SqlSpatialFunctions, System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 1);

            //EF6,System.Data.Entity.DbFunctionAttribute
            UpdatePredefinedTypes("System.Data.Entity.Core.Objects.EntityFunctions, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            UpdatePredefinedTypes("System.Data.Entity.DbFunctions, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            UpdatePredefinedTypes("System.Data.Entity.SqlServer.SqlFunctions, EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
            UpdatePredefinedTypes("System.Data.Entity.SqlServer.SqlSpatialFunctions, EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 2);
#endif
            // detect NumberDecimalSeparator from current culture
            //NumberFormatInfo numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
            //NumberDecimalSeparatorFromCulture = numberFormatInfo.NumberDecimalSeparator[0];
        }

        private static void UpdatePredefinedTypes(string typeName, int x)
        {
            try
            {
                Type efType = Type.GetType(typeName);
                if (efType != null)
                    _predefinedTypes.Add(efType, x);
            }
            catch
            {
                // in case of exception, do not add
            }
        }

        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (_keywords == null) _keywords = CreateKeywords();
            _symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _literals = new Dictionary<Expression, string>();
            if (parameters != null) ProcessParameters(parameters);
            if (values != null) ProcessValues(values);
            _text = expression;
            _textLen = _text.Length;
            SetTextPos(0);
            NextToken();
        }

        void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression pe in parameters)
                if (!string.IsNullOrEmpty(pe.Name))
                    AddSymbol(pe.Name, pe);
            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0].Name))
            {
                _parent = _it;
                _it = parameters[0];
                if (_root == null)
                    _root = _it;
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
                throw ParseError(Res.DuplicateIdentifier, name);
            _symbols.Add(name, value);
        }

        private Type _resultType;
        private bool _createParameterCtor;
        public Expression Parse(Type resultType, bool createParameterCtor)
        {
            _resultType = resultType;
            _createParameterCtor = createParameterCtor;

            int exprPos = _token.pos;
            Expression expr = ParseExpression();
            if (resultType != null)
                if ((expr = PromoteExpression(expr, resultType, true)) == null)
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType));
            ValidateToken(TokenId.End, Res.SyntaxError);
            return expr;
        }

#pragma warning disable 0219
        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            var orderings = new List<DynamicOrdering>();
            while (true)
            {
                Expression expr = ParseExpression();
                bool ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    NextToken();
                    ascending = false;
                }

                string methodName;
                if (orderings.Count == 0)
                    methodName = ascending ? methodOrderBy : methodOrderByDescending;
                else
                    methodName = ascending ? methodThenBy : methodThenByDescending;

                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending, MethodName = methodName });

                if (_token.id != TokenId.Comma)
                    break;

                NextToken();
            }

            ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }
#pragma warning restore 0219

        // ?: operator
        Expression ParseExpression()
        {
            int errorPos = _token.pos;
            Expression expr = ParseNullCoalescing();
            if (_token.id == TokenId.Question)
            {
                NextToken();
                Expression expr1 = ParseExpression();
                ValidateToken(TokenId.Colon, Res.ColonExpected);
                NextToken();
                Expression expr2 = ParseExpression();
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        // ?? (null-coalescing) operator
        Expression ParseNullCoalescing()
        {
            Expression expr = ParseConditionalOr();
            if (_token.id == TokenId.NullCoalescing)
            {
                NextToken();
                Expression right = ParseExpression();
                expr = Expression.Coalesce(expr, right);
            }
            return expr;
        }

        // isnull(a,b) operator
        Expression ParseIsNull()
        {
            int errorPos = _token.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 2)
                throw ParseError(errorPos, Res.IsNullRequiresTwoArgs);

            return Expression.Coalesce(args[0], args[1]);
        }

        // ||, or operator
        Expression ParseConditionalOr()
        {
            Expression left = ParseConditionalAnd();
            while (_token.id == TokenId.DoubleBar || TokenIdentifierIs("or"))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseConditionalAnd();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        // &&, and operator
        Expression ParseConditionalAnd()
        {
            Expression left = ParseIn();
            while (_token.id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseComparison();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        // in operator for literals - example: "x in (1,2,3,4)"
        // in operator to mimic contains - example: "x in @0", compare to @0.contains(x)
        // Adapted from ticket submitted by github user mlewis9548 
        Expression ParseIn()
        {
            Expression left = ParseLogicalAndOr();
            Expression accumulate = left;

            while (TokenIdentifierIs("in"))
            {
                var op = _token;

                NextToken();
                if (_token.id == TokenId.OpenParen) //literals (or other inline list)
                {
                    Expression identifier = left;
                    while (_token.id != TokenId.CloseParen)
                    {
                        NextToken();

                        //we need to parse unary expressions because otherwise 'in' clause will fail in use cases like 'in (-1, -1)' or 'in (!true)'
                        Expression right = ParseUnary();

                        //check for direct type match
                        if (identifier.Type != right.Type)
                        {
                            //check for nullable type match

                            if (!identifier.Type.GetTypeInfo().IsGenericType || identifier.Type.GetGenericTypeDefinition() != typeof(Nullable<>)
                                || identifier.Type.GetTypeInfo().GetGenericTypeArguments()[0] != right.Type)
                            {
                                throw ParseError(op.pos, Res.ExpressionTypeMismatch, identifier.Type);
                            }
                        }

                        CheckAndPromoteOperands(typeof(IEqualitySignatures), "==", ref identifier, ref right, op.pos);

                        if (accumulate.Type != typeof(bool))
                        {
                            accumulate = GenerateEqual(identifier, right);
                        }
                        else
                        {
                            accumulate = Expression.OrElse(accumulate, GenerateEqual(identifier, right));
                        }

                        if (_token.id == TokenId.End) throw ParseError(op.pos, Res.CloseParenOrCommaExpected);
                    }
                }
                else if (_token.id == TokenId.Identifier) //a single argument
                {
                    Expression right = ParsePrimary();

                    if (!typeof(IEnumerable).IsAssignableFrom(right.Type))
                        throw ParseError(_token.pos, Res.IdentifierImplementingInterfaceExpected, typeof(IEnumerable));

                    var args = new[] { left };

                    MethodBase containsSignature;
                    if (FindMethod(typeof(IEnumerableSignatures), "Contains", false, args, out containsSignature) != 1)
                        throw ParseError(op.pos, Res.NoApplicableAggregate, "Contains");

                    var typeArgs = new[] { left.Type };
                    args = new[] { right, left };

                    accumulate = Expression.Call(typeof(Enumerable), containsSignature.Name, typeArgs, args);
                }
                else
                    throw ParseError(op.pos, Res.OpenParenOrIdentifierExpected);

                NextToken();
            }

            return accumulate;
        }

        // &, | bitwise operators
        Expression ParseLogicalAndOr()
        {
            Expression left = ParseComparison();
            while (_token.id == TokenId.Amphersand || _token.id == TokenId.Bar)
            {
                Token op = _token;
                NextToken();
                Expression right = ParseComparison();

                if (left.Type.GetTypeInfo().IsEnum)
                {
                    left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));
                }

                if (right.Type.GetTypeInfo().IsEnum)
                {
                    right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));
                }

                switch (op.id)
                {
                    case TokenId.Amphersand:
                        // When at least one side of the operator is a string, consider it's a VB-style concatenation operator.
                        // Doesn't break any other function since logical AND with a string is invalid anyway.
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            left = GenerateStringConcat(left, right);
                        }
                        else
                        {
                            left = Expression.And(left, right);
                        }
                        break;
                    case TokenId.Bar:
                        left = Expression.Or(left, right);
                        break;
                }
            }
            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        Expression ParseComparison()
        {
            Expression left = ParseShift();
            while (_token.id == TokenId.Equal || _token.id == TokenId.DoubleEqual ||
                   _token.id == TokenId.ExclamationEqual || _token.id == TokenId.LessGreater ||
                   _token.id == TokenId.GreaterThan || _token.id == TokenId.GreaterThanEqual ||
                   _token.id == TokenId.LessThan || _token.id == TokenId.LessThanEqual)
            {
                Token op = _token;
                NextToken();
                Expression right = ParseShift();
                bool isEquality = op.id == TokenId.Equal || op.id == TokenId.DoubleEqual || op.id == TokenId.ExclamationEqual;
                if (isEquality && ((!left.Type.GetTypeInfo().IsValueType && !right.Type.GetTypeInfo().IsValueType) || (left.Type == typeof(Guid) && right.Type == typeof(Guid))))
                {
                    if (left.Type != right.Type)
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
                            throw IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        ConstantExpression constantExpr;

                        Expression e;
                        if ((e = PromoteExpression(right, left.Type, true)) != null)
                        {
                            right = e;
                        }
                        else if ((e = PromoteExpression(left, right.Type, true)) != null)
                        {
                            left = e;
                        }
                        else if (IsEnumType(left.Type) && (constantExpr = right as ConstantExpression) != null)
                        {
                            object wrt = null;
                            if (constantExpr.Value is string)
                            {
                                wrt = Enum.Parse(left.Type, constantExpr.Value as string, false);
                            }
                            else
                            {
                                wrt = Enum.ToObject(left.Type, constantExpr.Value);
                            }
                            right = Expression.Constant(wrt, left.Type);
                        }
                        else if (IsEnumType(right.Type) && (constantExpr = left as ConstantExpression) != null)
                        {
                            object wrt = null;
                            if (constantExpr.Value is string)
                            {
                                wrt = Enum.Parse(right.Type, constantExpr.Value as string, false);
                            }
                            else
                            {
                                wrt = Enum.ToObject(right.Type, constantExpr.Value);
                            }
                            left = Expression.Constant(wrt, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                    }
                }
                else
                {
                    CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures),
                        op.text, ref left, ref right, op.pos);
                }
                switch (op.id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = GenerateLessThanEqual(left, right);
                        break;
                }
            }
            return left;
        }

        // <<, >> operators
        Expression ParseShift()
        {
            Expression left = ParseAdditive();
            while (_token.id == TokenId.DoubleLessThan || _token.id == TokenId.DoubleGreaterThan)
            {
                Token op = _token;
                NextToken();
                Expression right = ParseAdditive();
                switch (op.id)
                {
                    case TokenId.DoubleLessThan:
                        CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
                        left = Expression.LeftShift(left, right);
                        break;
                    case TokenId.DoubleGreaterThan:
                        CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
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
            while (_token.id == TokenId.Plus || _token.id == TokenId.Minus)
            {
                Token op = _token;
                NextToken();
                Expression right = ParseMultiplicative();
                switch (op.id)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            left = GenerateStringConcat(left, right);
                        }
                        else
                        {
                            CheckAndPromoteOperands(typeof(IAddSignatures), op.text, ref left, ref right, op.pos);
                            left = GenerateAdd(left, right);
                        }
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.text, ref left, ref right, op.pos);
                        left = GenerateSubtract(left, right);
                        break;
                }
            }
            return left;
        }

        // *, /, %, mod operators
        Expression ParseMultiplicative()
        {
            Expression left = ParseUnary();
            while (_token.id == TokenId.Asterisk || _token.id == TokenId.Slash ||
                _token.id == TokenId.Percent || TokenIdentifierIs("mod"))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseUnary();
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
                switch (op.id)
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
            if (_token.id == TokenId.Minus || _token.id == TokenId.Exclamation || TokenIdentifierIs("not"))
            {
                Token op = _token;
                NextToken();
                if (op.id == TokenId.Minus && (_token.id == TokenId.IntegerLiteral || _token.id == TokenId.RealLiteral))
                {
                    _token.text = "-" + _token.text;
                    _token.pos = op.pos;
                    return ParsePrimary();
                }
                Expression expr = ParseUnary();
                if (op.id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.text, ref expr, op.pos);
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
                if (_token.id == TokenId.Dot)
                {
                    NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (_token.id == TokenId.OpenBracket)
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
            switch (_token.id)
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
            ValidateToken(TokenId.StringLiteral);
            char quote = _token.text[0];
            string s = _token.text.Substring(1, _token.text.Length - 2);
            int start = 0;
            while (true)
            {
                int i = s.IndexOf(quote, start);
                if (i < 0) break;
                s = s.Remove(i, 1);
                start = i + 1;
            }
            if (quote == '\'')
            {
                if (s.Length != 1)
                    throw ParseError(Res.InvalidCharacterLiteral);
                NextToken();
                return CreateLiteral(s[0], s);
            }
            NextToken();
            return CreateLiteral(s, s);
        }

        Expression ParseIntegerLiteral()
        {
            ValidateToken(TokenId.IntegerLiteral);

            string text = _token.text;
            string qualifier = null;
            char last = text[text.Length - 1];

            if (char.IsLetter(last))
            {
                int pos = text.Length - 1, count = 0;
                while (char.IsLetter(text[pos]))
                {
                    ++count;
                    --pos;
                }
                qualifier = text.Substring(text.Length - count, count);
                text = text.Substring(0, text.Length - count);
            }

            if (text[0] != '-')
            {
                ulong value;
                if (!ulong.TryParse(text, out value))
                    throw ParseError(Res.InvalidIntegerLiteral, text);

                NextToken();
                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "U" || qualifier == "u") return CreateLiteral((uint)value, text);
                    if (qualifier == "L" || qualifier == "l") return CreateLiteral((long)value, text);

                    // in case of UL, just return
                    return CreateLiteral(value, text);
                }

                if (value <= int.MaxValue) return CreateLiteral((int)value, text);
                if (value <= uint.MaxValue) return CreateLiteral((uint)value, text);
                if (value <= long.MaxValue) return CreateLiteral((long)value, text);

                return CreateLiteral(value, text);
            }
            else
            {
                long value;
                if (!long.TryParse(text, out value))
                    throw ParseError(Res.InvalidIntegerLiteral, text);

                NextToken();
                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "L" || qualifier == "l")
                        return CreateLiteral(value, text);

                    if (qualifier == "F" || qualifier == "f")
                        return TryParseAsFloat(text, qualifier[0]);

                    if (qualifier == "D" || qualifier == "d")
                        return TryParseAsDouble(text, qualifier[0]);

                    throw ParseError(Res.MinusCannotBeAppliedToUnsignedInteger);
                }

                if (value <= int.MaxValue) return CreateLiteral((int)value, text);

                return CreateLiteral(value, text);
            }
        }

        Expression ParseRealLiteral()
        {
            ValidateToken(TokenId.RealLiteral);

            string text = _token.text;
            char qualifier = text[text.Length - 1];

            NextToken();
            return TryParseAsFloat(text, qualifier);
        }

        Expression TryParseAsFloat(string text, char qualifier)
        {
            if (qualifier == 'F' || qualifier == 'f')
            {
                float f;
                if (float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out f))
                {
                    return CreateLiteral(f, text);
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
                    return CreateLiteral(d, text);
                }
            }

            if (double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
            {
                return CreateLiteral(d, text);
            }

            throw ParseError(Res.InvalidRealLiteral, text);
        }

        Expression CreateLiteral(object value, string text)
        {
            ConstantExpression expr = Expression.Constant(value);
            _literals.Add(expr, text);
            return expr;
        }

        Expression ParseParenExpression()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            Expression e = ParseExpression();
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            NextToken();
            return e;
        }

        Expression ParseIdentifier()
        {
            ValidateToken(TokenId.Identifier);
            object value;

            if (_keywords.TryGetValue(_token.text, out value))
            {
                var typeValue = value as Type;
                if (typeValue != null) return ParseTypeAccess(typeValue);

                if (value == (object)KEYWORD_IT) return ParseIt();
                if (value == (object)KEYWORD_PARENT) return ParseParent();
                if (value == (object)KEYWORD_ROOT) return ParseRoot();
                if (value == (object)SYMBOL_IT) return ParseIt();
                if (value == (object)SYMBOL_PARENT) return ParseParent();
                if (value == (object)SYMBOL_ROOT) return ParseRoot();
                if (value == (object)KEYWORD_IIF) return ParseIif();
                if (value == (object)KEYWORD_NEW) return ParseNew();
                if (value == (object)KEYWORD_ISNULL) return ParseIsNull();

                NextToken();

                return (Expression)value;
            }

            if (_symbols.TryGetValue(_token.text, out value) ||
                _externals != null && _externals.TryGetValue(_token.text, out value))
            {
                Expression expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    LambdaExpression lambda = expr as LambdaExpression;
                    if (lambda != null) return ParseLambdaInvocation(lambda);
                }

                NextToken();

                return expr;
            }

            if (_it != null)
                return ParseMemberAccess(null, _it);

            throw ParseError(Res.UnknownIdentifier, _token.text);
        }

        Expression ParseIt()
        {
            if (_it == null)
                throw ParseError(Res.NoItInScope);
            NextToken();
            return _it;
        }

        Expression ParseParent()
        {
            if (_parent == null)
                throw ParseError(Res.NoParentInScope);
            NextToken();
            return _parent;
        }

        Expression ParseRoot()
        {
            if (_root == null)
                throw ParseError(Res.NoRootInScope);
            NextToken();
            return _root;
        }

        Expression ParseIif()
        {
            int errorPos = _token.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 3)
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            if (expr1.Type != expr2.Type)
            {
                Expression expr1As2 = expr2 != NullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
                Expression expr2As1 = expr1 != NullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
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
                    string type1 = expr1 != NullLiteral ? expr1.Type.Name : "null";
                    string type2 = expr2 != NullLiteral ? expr2.Type.Name : "null";
                    if (expr1As2 != null)
                        throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
                }
            }

            return Expression.Condition(test, expr1, expr2);
        }

        Expression ParseNew()
        {
            NextToken();
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();

            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();

            while (true)
            {
                int exprPos = _token.pos;
                Expression expr = ParseExpression();
                string propName;
                if (TokenIdentifierIs("as"))
                {
                    NextToken();
                    propName = GetIdentifier();
                    NextToken();
                }
                else
                {
                    if (!TryGetMemberName(expr, out propName))
                        throw ParseError(exprPos, Res.MissingAsClause);
                }

                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));

                if (_token.id != TokenId.Comma)
                    break;

                NextToken();
            }

            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();

            // http://solutionizing.net/category/linq/ 
            Type type = _resultType ?? DynamicClassFactory.CreateType(properties, _createParameterCtor);

            var propertyTypes = type.GetProperties().Select(p => p.PropertyType).ToArray();
            var ctor = type.GetConstructor(propertyTypes);
            if (ctor != null)
                return Expression.New(ctor, expressions);

            MemberBinding[] bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < bindings.Length; i++)
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            int errorPos = _token.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            MethodBase method;
            if (FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            return Expression.Invoke(lambda, args);
        }

        Expression ParseTypeAccess(Type type)
        {
            int errorPos = _token.pos;
            NextToken();
            if (_token.id == TokenId.Question)
            {
                if (!type.GetTypeInfo().IsValueType || IsNullableType(type))
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type));
                type = typeof(Nullable<>).MakeGenericType(type);
                NextToken();
            }

            // This is a shorthand for explicitely converting a string to something
            bool shorthand = _token.id == TokenId.StringLiteral;
            if (_token.id == TokenId.OpenParen || shorthand)
            {
                Expression[] args = shorthand
                    ? new[] { ParseStringLiteral() }
                    : ParseArgumentList();

                MethodBase method;
                switch (FindBestMethod(type.GetConstructors(), args, out method))
                {
                    case 0:
                        if (args.Length == 1)
                            return GenerateConversion(args[0], type, errorPos);
                        throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type));
                    case 1:
                        return Expression.New((ConstructorInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type));
                }
            }
            ValidateToken(TokenId.Dot, Res.DotOrOpenParenOrStringLiteralExpected);
            NextToken();

            return ParseMemberAccess(type, null);
        }

        static Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            Type exprType = expr.Type;
            if (exprType == type)
                return expr;

            if (exprType.GetTypeInfo().IsValueType && type.GetTypeInfo().IsValueType)
            {
                if ((IsNullableType(exprType) || IsNullableType(type)) &&
                    GetNonNullableType(exprType) == GetNonNullableType(type))
                    return Expression.Convert(expr, type);

                if ((IsNumericType(exprType) || IsEnumType(exprType)) &&
                    (IsNumericType(type)) || IsEnumType(type))
                    return Expression.ConvertChecked(expr, type);
            }

            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.GetTypeInfo().IsInterface || type.GetTypeInfo().IsInterface)
                return Expression.Convert(expr, type);

            // Try to Parse the string rather that just generate the convert statement
            if (expr.NodeType == ExpressionType.Constant && exprType == typeof(string))
            {
                string text = (string)((ConstantExpression)expr).Value;

                // DateTime is parsed as UTC time.
                DateTime dateTime;
                if (type == typeof(DateTime) && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return Expression.Constant(dateTime, type);

                object[] arguments = { text, null };
#if NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD
                MethodInfo method = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
#else
                MethodInfo method = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), type.MakeByRefType() }, null);
#endif
                if (method != null && (bool)method.Invoke(null, arguments))
                    return Expression.Constant(arguments[1], type);
            }

            throw ParseError(errorPos, Res.CannotConvertValue, GetTypeName(exprType), GetTypeName(type));
        }

        Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null) type = instance.Type;
            int errorPos = _token.pos;
            string id = GetIdentifier();
            NextToken();

            if (_token.id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    Type enumerableType = FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != null)
                    {
                        Type elementType = enumerableType.GetTypeInfo().GetGenericTypeArguments()[0];
                        return ParseAggregate(instance, elementType, id, errorPos);
                    }
                }

                Expression[] args = ParseArgumentList();
                MethodBase mb;
                switch (FindMethod(type, id, instance == null, args, out mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableMethod, id, GetTypeName(type));
                    case 1:
                        MethodInfo method = (MethodInfo)mb;
                        if (!IsPredefinedType(method.DeclaringType))
                            throw ParseError(errorPos, Res.MethodsAreInaccessible, GetTypeName(method.DeclaringType));

                        if (method.ReturnType == typeof(void))
                            throw ParseError(errorPos, Res.MethodIsVoid, id, GetTypeName(method.DeclaringType));

                        return Expression.Call(instance, method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, GetTypeName(type));
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
                throw ParseError(errorPos, Res.UnknownPropertyOrField, id, GetTypeName(type));
            }

            var property = member as PropertyInfo;
            if (property != null)
                return Expression.Property(instance, property);

            return Expression.Field(instance, (FieldInfo)member);
        }

        static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == generic)
                    return type;
                if (generic.GetTypeInfo().IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = FindGenericType(generic, intfType);
                        if (found != null) return found;
                    }
                }
                type = type.GetTypeInfo().BaseType;
            }
            return null;
        }

        Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            var oldParent = _parent;

            ParameterExpression outerIt = _it;
            ParameterExpression innerIt = Expression.Parameter(elementType, "");

            _parent = _it;

            if (methodName == "Contains" || methodName == "Skip" || methodName == "Take")
            {
                //for any method that acts on the parent element type, we need to specify the outerIt as scope.
                _it = outerIt;
            }
            else
            {
                _it = innerIt;
            }

            Expression[] args = ParseArgumentList();

            _it = outerIt;
            _parent = oldParent;

            MethodBase signature;
            if (FindMethod(typeof(IEnumerableSignatures), methodName, false, args, out signature) != 1)
                throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);

            Type[] typeArgs;
            if ((new[] { "Min", "Max", "Select", "OrderBy", "OrderByDescending" }).Contains(signature.Name))
            {
                typeArgs = new[] { elementType, args[0].Type };
            }
            else if (signature.Name == "SelectMany")
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

            if (signature.Name == "Contains" || signature.Name == "Take" || signature.Name == "Skip")
            {
                args = new[] { instance, args[0] };
            }
            else if (args.Length == 0)
            {
                args = new[] { instance };
            }
            else
            {
                args = new[] { instance, Expression.Lambda(args[0], innerIt) };
            }

            return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args);
        }

        Expression[] ParseArgumentList()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            Expression[] args = _token.id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            return args;
        }

        Expression[] ParseArguments()
        {
            List<Expression> argList = new List<Expression>();
            while (true)
            {
                argList.Add(ParseExpression());
                if (_token.id != TokenId.Comma) break;
                NextToken();
            }
            return argList.ToArray();
        }

        Expression ParseElementAccess(Expression expr)
        {
            int errorPos = _token.pos;
            ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            NextToken();
            Expression[] args = ParseArguments();
            ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            NextToken();
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                Expression index = PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                    throw ParseError(errorPos, Res.InvalidIndex);
                return Expression.ArrayIndex(expr, index);
            }
            else
            {
                MethodBase mb;
                switch (FindIndexer(expr.Type, args, out mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableIndexer,
                            GetTypeName(expr.Type));
                    case 1:
                        return Expression.Call(expr, (MethodInfo)mb, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousIndexerInvocation,
                            GetTypeName(expr.Type));
                }
            }
        }

        static bool IsPredefinedType(Type type)
        {
            if (_predefinedTypes.ContainsKey(type)) return true;

            if (GlobalConfig.CustomTypeProvider != null && GlobalConfig.CustomTypeProvider.GetCustomTypes().Contains(type)) return true;

            return false;
        }

        static bool IsNullableType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetTypeInfo().GetGenericTypeArguments()[0] : type;
        }

        static string GetTypeName(Type type)
        {
            Type baseType = GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

        static bool TryGetMemberName(Expression expression, out string memberName)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null && expression.NodeType == ExpressionType.Coalesce)
            {
                memberExpression = ((expression as BinaryExpression).Left) as MemberExpression;
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
            //#if !NET35
            //            var dynamicExpression = expression as Expressions.DynamicExpression;
            //            if (dynamicExpression != null)
            //            {
            //                memberName = ((GetMemberBinder)dynamicExpression.Binder).Name;
            //                return true;
            //            }
            //#endif

            memberName = null;
            return false;
        }

        static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (type.GetTypeInfo().IsEnum) return 0;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
#else
            if (type.GetTypeInfo().IsEnum) return 0;

            if (type == typeof(Char) || type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal))
                return 1;
            if (type == typeof(SByte) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
                return 2;
            if (type == typeof(Byte) || type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64))
                return 3;
            return 0;
#endif
        }

        static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).GetTypeInfo().IsEnum;
        }

        void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            Expression[] args = { expr };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
                throw ParseError(errorPos, Res.IncompatibleOperand, opName, GetTypeName(args[0].Type));
            expr = args[0];
        }

        void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            Expression[] args = { left, right };

            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            left = args[0];
            right = args[1];
        }

        static Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
        {
            return ParseError(pos, Res.IncompatibleOperands, opName, GetTypeName(left.Type), GetTypeName(right.Type));
        }

        static MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0)
                    return members[0];
            }
            return null;
#else
            foreach (Type t in SelfAndBaseTypes(type))
            {
                // Try to find a property with the specified memberName
                MemberInfo member = t.GetTypeInfo().DeclaredProperties.FirstOrDefault(x => x.Name.ToLowerInvariant() == memberName.ToLowerInvariant());
                if (member != null)
                    return member;

                // If no property is found, try to get a field with the specified memberName
                member = t.GetTypeInfo().DeclaredFields.FirstOrDefault(x => (x.IsStatic || !staticAccess) && x.Name.ToLowerInvariant() == memberName.ToLowerInvariant());
                if (member != null)
                    return member;

                // No property or field is found, try the base type.
            }
            return null;
#endif
        }

        int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
        {
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Method,
                    flags, Type.FilterNameIgnoreCase, methodName);
                int count = FindBestMethod(members.Cast<MethodBase>(), args, out method);
                if (count != 0) return count;
            }
            method = null;
            return 0;
#else
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MethodInfo[] methods = t.GetTypeInfo().DeclaredMethods.Where(x => (x.IsStatic || !staticAccess) && x.Name.ToLowerInvariant() == methodName.ToLowerInvariant()).ToArray();
                int count = FindBestMethod(methods, args, out method);
                if (count != 0) return count;
            }

            method = null;
            return 0;
#endif
        }

        int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (Type t in SelfAndBaseTypes(type))
            {
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
                MemberInfo[] members = t.GetDefaultMembers();
#else
                MemberInfo[] members = new MemberInfo[0];
#endif
                if (members.Length != 0)
                {
                    IEnumerable<MethodBase> methods = members
                        .OfType<PropertyInfo>().
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
                        Select(p => (MethodBase)p.GetGetMethod()).
                        Where(m => m != null);
#else
                    Select(p => (MethodBase)p.GetMethod);
#endif

                    int count = FindBestMethod(methods, args, out method);
                    if (count != 0) return count;
                }
            }
            method = null;
            return 0;
        }

        static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type t in type.GetInterfaces()) AddInterface(types, t);
            }
        }

        class MethodData
        {
            public MethodBase MethodBase;
            public ParameterInfo[] Parameters;
            public Expression[] Args;
        }

        int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            MethodData[] applicable = methods.
                Select(m => new MethodData { MethodBase = m, Parameters = m.GetParameters() }).
                Where(m => IsApplicable(m, args)).
                ToArray();

            if (applicable.Length > 1)
            {
                applicable = applicable.Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).ToArray();
            }

            if (applicable.Length == 1)
            {
                MethodData md = applicable[0];
                for (int i = 0; i < args.Length; i++) args[i] = md.Args[i];
                method = md.MethodBase;
            }
            else
            {
                method = null;
            }

            return applicable.Length;
        }

        bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length) return false;
            Expression[] promotedArgs = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo pi = method.Parameters[i];
                if (pi.IsOut) return false;
                Expression promoted = PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null) return false;
                promotedArgs[i] = promoted;
            }
            method.Args = promotedArgs;
            return true;
        }

        Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type == type) return expr;

            var ce = expr as ConstantExpression;

            if (ce != null)
            {
                if (ce == NullLiteral)
                {
                    if (!type.GetTypeInfo().IsValueType || IsNullableType(type))
                        return Expression.Constant(null, type);
                }
                else
                {
                    string text;
                    if (_literals.TryGetValue(ce, out text))
                    {
                        Type target = GetNonNullableType(type);
                        object value = null;
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);

                                // Make sure an enum value stays an enum value
                                if (target.IsEnum)
                                    value = Enum.ToObject(target, value);
                                break;
                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = ParseNumber(text, target);
                                break;
                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }
#else
                        if (ce.Type == typeof(Int32) || ce.Type == typeof(UInt32) || ce.Type == typeof(Int64) || ce.Type == typeof(UInt64))
                            value = ParseNumber(text, target);
                        else if (ce.Type == typeof(Double))
                        {
                            if (target == typeof(decimal)) value = ParseNumber(text, target);
                        }
                        else if (ce.Type == typeof(String))
                            value = ParseEnum(text, target);
#endif
                        if (value != null)
                            return Expression.Constant(value, type);
                    }
                }
            }

            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.GetTypeInfo().IsValueType || exact)
                    return Expression.Convert(expr, type);

                return expr;
            }

            return null;
        }

        static object ParseNumber(string text, Type type)
        {
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb)) return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s)) return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us)) return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui)) return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul)) return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f)) return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d)) return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e)) return e;
                    break;
            }
#else
            var tp = GetNonNullableType(type);
            if (tp == typeof(SByte))
            {
                sbyte sb;
                if (sbyte.TryParse(text, out sb)) return sb;
            }
            else if (tp == typeof(Byte))
            {
                byte b;
                if (byte.TryParse(text, out b)) return b;
            }
            else if (tp == typeof(Int16))
            {
                short s;
                if (short.TryParse(text, out s)) return s;
            }
            else if (tp == typeof(UInt16))
            {
                ushort us;
                if (ushort.TryParse(text, out us)) return us;
            }
            else if (tp == typeof(Int32))
            {
                int i;
                if (int.TryParse(text, out i)) return i;
            }
            else if (tp == typeof(UInt32))
            {
                uint ui;
                if (uint.TryParse(text, out ui)) return ui;
            }
            else if (tp == typeof(Int64))
            {
                long l;
                if (long.TryParse(text, out l)) return l;
            }
            else if (tp == typeof(UInt64))
            {
                ulong ul;
                if (ulong.TryParse(text, out ul)) return ul;
            }
            else if (tp == typeof(Single))
            {
                float f;
                if (float.TryParse(text, out f)) return f;
            }
            else if (tp == typeof(Double))
            {
                double d;
                if (double.TryParse(text, out d)) return d;
            }
            else if (tp == typeof(Decimal))
            {
                decimal e;
                if (decimal.TryParse(text, out e)) return e;
            }
#endif
            return null;
        }

        static object ParseEnum(string name, Type type)
        {
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0) return ((FieldInfo)memberInfos[0]).GetValue(null);
            }
#else
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, name, true);
            }
#endif
            return null;
        }

        static bool IsCompatibleWith(Type source, Type target)
        {
#if !(NETFX_CORE ||WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            TypeCode sc = st.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
#else
            if (source == target) return true;
            if (!target.GetTypeInfo().IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            Type sc = st.GetTypeInfo().IsEnum ? typeof(Object) : st;
            Type tc = tt.GetTypeInfo().IsEnum ? typeof(Object) : tt;

            if (sc == typeof(SByte))
            {
                if (tc == typeof(SByte) || tc == typeof(Int16) || tc == typeof(Int32) || tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Byte))
            {
                if (tc == typeof(Byte) || tc == typeof(Int16) || tc == typeof(UInt16) || tc == typeof(Int32) || tc == typeof(UInt32) || tc == typeof(Int64) || tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Int16))
            {
                if (tc == typeof(Int16) || tc == typeof(Int32) || tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(UInt16))
            {
                if (tc == typeof(UInt16) || tc == typeof(Int32) || tc == typeof(UInt32) || tc == typeof(Int64) || tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Int32))
            {
                if (tc == typeof(Int32) || tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(UInt32))
            {
                if (tc == typeof(UInt32) || tc == typeof(Int64) || tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Int64))
            {
                if (tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(UInt64))
            {
                if (tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Single))
            {
                if (tc == typeof(Single) || tc == typeof(Double))
                    return true;
            }

            if (st == tt)
                return true;
            return false;
#endif
        }

        static bool IsBetterThan(Expression[] args, MethodData first, MethodData second)
        {
            bool better = false;
            for (int i = 0; i < args.Length; i++)
            {
                CompareConversionType result = CompareConversions(args[i].Type, first.Parameters[i].ParameterType, second.Parameters[i].ParameterType);

                // If second is better, return false
                if (result == CompareConversionType.Second)
                    return false;

                // If first is better, return true
                if (result == CompareConversionType.First)
                    return true;

                // If both are same, just set better to true and continue
                if (result == CompareConversionType.Both)
                    better = true;
            }

            return better;
        }

        enum CompareConversionType
        {
            Both = 0,
            First = 1,
            Second = -1
        }

        // Return "First" if s -> t1 is a better conversion than s -> t2
        // Return "Second" if s -> t2 is a better conversion than s -> t1
        // Return "Both" if neither conversion is better
        static CompareConversionType CompareConversions(Type source, Type first, Type second)
        {
            if (first == second) return CompareConversionType.Both;
            if (source == first) return CompareConversionType.First;
            if (source == second) return CompareConversionType.Second;

            bool firstIsCompatibleWithSecond = IsCompatibleWith(first, second);
            bool secondIsCompatibleWithFirst = IsCompatibleWith(second, first);

            if (firstIsCompatibleWithSecond && !secondIsCompatibleWithFirst) return CompareConversionType.First;
            if (secondIsCompatibleWithFirst && !firstIsCompatibleWithSecond) return CompareConversionType.Second;

            if (IsSignedIntegralType(first) && IsUnsignedIntegralType(second)) return CompareConversionType.First;
            if (IsSignedIntegralType(second) && IsUnsignedIntegralType(first)) return CompareConversionType.Second;

            return CompareConversionType.Both;
        }

        static Expression GenerateEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);
            return Expression.Equal(left, right);
        }

        static Expression GenerateNotEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);
            return Expression.NotEqual(left, right);
        }

        static Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                var leftPart = left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left;
                var rightPart = right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right;
                return Expression.GreaterThan(leftPart, rightPart);
            }

            return Expression.GreaterThan(left, right);
        }

        static Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.GreaterThanOrEqual(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            return Expression.GreaterThanOrEqual(left, right);
        }

        static Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.LessThan(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            return Expression.LessThan(left, right);
        }

        static Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.LessThanOrEqual(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            return Expression.LessThanOrEqual(left, right);
        }

        static Expression GenerateAdd(Expression left, Expression right)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }

        static Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        static Expression GenerateStringConcat(Expression left, Expression right)
        {
            // Allow concat String with something else
            return Expression.Call(null, typeof(string).GetMethod("Concat", new[] { left.Type, right.Type }), new[] { left, right });
        }

        static MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        static Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        static void OptimizeForEqualityIfPossible(ref Expression left, ref Expression right)
        {
            // The goal here is to provide the way to convert some types from the string form in a way that is compatible with Linq to Entities.
            //
            // The Expression.Call(typeof(Guid).GetMethod("Parse"), right); does the job only for Linq to Object but Linq to Entities.
            //
            Type leftType = left.Type, rightType = right.Type;
            if (rightType == typeof(string) && right.NodeType == ExpressionType.Constant)
            {
                right = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)right).Value, leftType) ?? right;
            }

            if (leftType == typeof(string) && left.NodeType == ExpressionType.Constant)
            {
                left = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)left).Value, rightType) ?? left;
            }
        }

        static Expression OptimizeStringForEqualityIfPossible(string text, Type type)
        {
            DateTime dateTime;

            if (type == typeof(DateTime) &&
                DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                return Expression.Constant(dateTime, typeof(DateTime));
#if !NET35
            Guid guid;
            if (type == typeof(Guid) && Guid.TryParse(text, out guid))
                return Expression.Constant(guid, typeof(Guid));
#else
            try
            {
                return Expression.Constant(new Guid(text));
            }
            catch
            {
                //Doing it in old fashion way when no TryParse interface was provided by .NET
            }
#endif
            return null;
        }

        void SetTextPos(int pos)
        {
            _textPos = pos;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        void NextChar()
        {
            if (_textPos < _textLen) _textPos++;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        void NextToken()
        {
            while (char.IsWhiteSpace(_ch)) NextChar();
            TokenId t;
            int tokenPos = _textPos;
            switch (_ch)
            {
                case '!':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (_ch == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else if (_ch == '<')
                    {
                        NextChar();
                        t = TokenId.DoubleLessThan;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        t = TokenId.DoubleGreaterThan;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    if (_ch == '?')
                    {
                        NextChar();
                        t = TokenId.NullCoalescing;
                    }
                    else
                    {
                        t = TokenId.Question;
                    }
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (_ch == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    char quote = _ch;
                    do
                    {
                        NextChar();
                        while (_textPos < _textLen && _ch != quote) NextChar();
                        if (_textPos == _textLen)
                            throw ParseError(_textPos, Res.UnterminatedStringLiteral);
                        NextChar();
                    } while (_ch == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (char.IsLetter(_ch) || _ch == '@' || _ch == '_' || _ch == '$' || _ch == '^' || _ch == '~')
                    {
                        do
                        {
                            NextChar();
                        } while (char.IsLetterOrDigit(_ch) || _ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }

                    if (char.IsDigit(_ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (char.IsDigit(_ch));

                        if (_ch == 'U' || _ch == 'L')
                        {
                            NextChar();
                            if (_ch == 'L')
                            {
                                if (_text[_textPos - 1] == 'U') NextChar();
                                else throw ParseError(_textPos, Res.InvalidIntegerQualifier, _text.Substring(_textPos - 1, 2));
                            }
                            ValidateExpression();
                            break;
                        }

                        if (_ch == NumberDecimalSeparator)
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'E' || _ch == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (_ch == '+' || _ch == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'F' || _ch == 'f') NextChar();
                        if (_ch == 'D' || _ch == 'd') NextChar();
                        break;
                    }

                    if (_textPos == _textLen)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(_textPos, Res.InvalidCharacter, _ch);
            }

            _token.pos = tokenPos;
            _token.text = _text.Substring(tokenPos, _textPos - tokenPos);
            _token.id = GetAliasedTokenId(t, _token.text);
        }

        bool TokenIdentifierIs(string id)
        {
            return _token.id == TokenId.Identifier && string.Equals(id, _token.text, StringComparison.OrdinalIgnoreCase);
        }

        string GetIdentifier()
        {
            ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            string id = _token.text;
            if (id.Length > 1 && id[0] == '@') id = id.Substring(1);
            return id;
        }

        void ValidateExpression()
        {
            if (char.IsLetterOrDigit(_ch)) throw ParseError(_textPos, Res.ExpressionExpected);
        }

        void ValidateDigit()
        {
            if (!char.IsDigit(_ch)) throw ParseError(_textPos, Res.DigitExpected);
        }

        void ValidateToken(TokenId t, string errorMessage)
        {
            if (_token.id != t) throw ParseError(errorMessage);
        }

        void ValidateToken(TokenId t)
        {
            if (_token.id != t) throw ParseError(Res.SyntaxError);
        }

        Exception ParseError(string format, params object[] args)
        {
            return ParseError(_token.pos, format, args);
        }

        static Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }

        static Dictionary<string, object> CreateKeywords()
        {
            var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                {"true", TrueLiteral},
                {"false", FalseLiteral},
                {"null", NullLiteral}
            };

            if (GlobalConfig.AreContextKeywordsEnabled)
            {
                d.Add(KEYWORD_IT, KEYWORD_IT);
                d.Add(KEYWORD_PARENT, KEYWORD_PARENT);
                d.Add(KEYWORD_ROOT, KEYWORD_ROOT);
            }

            d.Add(SYMBOL_IT, SYMBOL_IT);
            d.Add(SYMBOL_PARENT, SYMBOL_PARENT);
            d.Add(SYMBOL_ROOT, SYMBOL_ROOT);
            d.Add(KEYWORD_IIF, KEYWORD_IIF);
            d.Add(KEYWORD_NEW, KEYWORD_NEW);
            d.Add(KEYWORD_ISNULL, KEYWORD_ISNULL);

            foreach (Type type in _predefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                d[type.FullName] = type;
                d[type.Name] = type;
            }

            foreach (KeyValuePair<string, Type> pair in _predefinedTypesShorthands)
                d.Add(pair.Key, pair.Value);

            if (GlobalConfig.CustomTypeProvider != null)
            {
                foreach (Type type in GlobalConfig.CustomTypeProvider.GetCustomTypes())
                {
                    d[type.FullName] = type;
                    d[type.Name] = type;
                }
            }

            return d;
        }

        static TokenId GetAliasedTokenId(TokenId t, string alias)
        {
            TokenId id;
            return t == TokenId.Identifier && _predefinedAliases.TryGetValue(alias, out id) ? id : t;
        }

        internal static void ResetDynamicLinqTypes()
        {
            _keywords = null;
        }
    }
}
