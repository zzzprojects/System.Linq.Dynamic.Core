using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class ExpressionHelper
    {
        public static void ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref Expression left, ref Expression right)
        {
            if (left.Type == right.Type)
            {
                return;
            }

            if (left.Type == typeof(UInt64) || right.Type == typeof(UInt64))
            {
                right = right.Type != typeof(UInt64) ? Expression.Convert(right, typeof(UInt64)) : right;
                left = left.Type != typeof(UInt64) ? Expression.Convert(left, typeof(UInt64)) : left;
            }
            else if (left.Type == typeof(Int64) || right.Type == typeof(Int64))
            {
                right = right.Type != typeof(Int64) ? Expression.Convert(right, typeof(Int64)) : right;
                left = left.Type != typeof(Int64) ? Expression.Convert(left, typeof(Int64)) : left;
            }
            else if (left.Type == typeof(UInt32) || right.Type == typeof(UInt32))
            {
                right = right.Type != typeof(UInt32) ? Expression.Convert(right, typeof(UInt32)) : right;
                left = left.Type != typeof(UInt32) ? Expression.Convert(left, typeof(UInt32)) : left;
            }
            else if (left.Type == typeof(Int32) || right.Type == typeof(Int32))
            {
                right = right.Type != typeof(Int32) ? Expression.Convert(right, typeof(Int32)) : right;
                left = left.Type != typeof(Int32) ? Expression.Convert(left, typeof(Int32)) : left;
            }
            else if (left.Type == typeof(UInt16) || right.Type == typeof(UInt16))
            {
                right = right.Type != typeof(UInt16) ? Expression.Convert(right, typeof(UInt16)) : right;
                left = left.Type != typeof(UInt16) ? Expression.Convert(left, typeof(UInt16)) : left;
            }
            else if (left.Type == typeof(Int16) || right.Type == typeof(Int16))
            {
                right = right.Type != typeof(Int16) ? Expression.Convert(right, typeof(Int16)) : right;
                left = left.Type != typeof(Int16) ? Expression.Convert(left, typeof(Int16)) : left;
            }
            else if (left.Type == typeof(Byte) || right.Type == typeof(Byte))
            {
                right = right.Type != typeof(Byte) ? Expression.Convert(right, typeof(Byte)) : right;
                left = left.Type != typeof(Byte) ? Expression.Convert(left, typeof(Byte)) : left;
            }
        }

        public static Expression GenerateAdd(Expression left, Expression right)
        {
            return Expression.Add(left, right);
        }

        public static Expression GenerateStringConcat(Expression left, Expression right)
        {
            return GenerateStaticMethodCall("Concat", left, right);
        }

        public static Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        public static Expression GenerateEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);
            return Expression.Equal(left, right);
        }

        public static Expression GenerateNotEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);
            return Expression.NotEqual(left, right);
        }

        public static Expression GenerateGreaterThan(Expression left, Expression right)
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

        public static Expression GenerateGreaterThanEqual(Expression left, Expression right)
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

        public static Expression GenerateLessThan(Expression left, Expression right)
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

        public static Expression GenerateLessThanEqual(Expression left, Expression right)
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

        public static void OptimizeForEqualityIfPossible(ref Expression left, ref Expression right)
        {
            // The goal here is to provide the way to convert some types from the string form in a way that is compatible with Linq to Entities.
            //
            // The Expression.Call(typeof(Guid).GetMethod("Parse"), right); does the job only for Linq to Object but Linq to Entities.
            //
            Type leftType = left.Type;
            Type rightType = right.Type;

            if (rightType == typeof(string) && right.NodeType == ExpressionType.Constant)
            {
                right = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)right).Value, leftType) ?? right;
            }

            if (leftType == typeof(string) && left.NodeType == ExpressionType.Constant)
            {
                left = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)left).Value, rightType) ?? left;
            }
        }

        public static Expression OptimizeStringForEqualityIfPossible(string text, Type type)
        {
            if (type == typeof(DateTime) && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return Expression.Constant(dateTime, typeof(DateTime));
            }
#if !NET35
            if (type == typeof(Guid) && Guid.TryParse(text, out Guid guid))
            {
                return Expression.Constant(guid, typeof(Guid));
            }
#else
            try
            {
                return Expression.Constant(new Guid(text));
            }
            catch
            {
                // Doing it in old fashion way when no TryParse interface was provided by .NET
            }
#endif
            return null;
        }

        static MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        static Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }
    }
}
