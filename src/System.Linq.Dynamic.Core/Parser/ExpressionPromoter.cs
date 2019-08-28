using ETG.SABENTISpro.Utils.DynamicLinkCore.Compatibility;
using System.Linq.Expressions;
using System.Reflection;
using TypeLite.Extensions;
using TypeExtensions = ETG.SABENTISpro.Utils.HelpersUtils.TypeExtensions;

namespace System.Linq.Dynamic.Core.Parser
{
    public class ExpressionPromoter : IExpressionPromoter
    {
        /// <inheritdoc cref="IExpressionPromoter.Promote(Expression, Type, bool, bool)"/>
        public virtual Expression Promote(Expression expr, Type type, bool exact, bool convertExpr)
        {
            return this.Promote(expr, type, exact, convertExpr, out _);
        }

        /// <inheritdoc cref="IExpressionPromoter.Promote(Expression, Type, bool, bool)"/>
        public virtual Expression Promote(Expression expr, Type type, bool exact, bool convertExpr, out Type promotedTarget)
        {
            promotedTarget = null;

            if (expr.Type == type)
            {
                return expr;
            }

            if (expr is LambdaExpression le)
            {
                if (le.GetReturnType() == type)
                {
                    return expr;
                }

                if (type.IsNullable() && le.ReturnType == TypeExtensions.GetUnderlyingType(type))
                {
                    // Boxing
                    var boxed = Expression.Convert(le.Body, type);
                    Type delegateType;

                    // TODO: Recode to handle N possible arguments
                    switch (le.Parameters.Count)
                    {
                        case 1:
                            delegateType = typeof(Func<,>).MakeGenericType(le.Parameters[0].Type, type);
                            break;
                        case 2:
                            delegateType = typeof(Func<,,>).MakeGenericType(le.Parameters[0].Type, le.Parameters[1].Type, type);
                            break;
                        case 3:
                            delegateType = typeof(Func<,,,>).MakeGenericType(le.Parameters[0].Type, le.Parameters[1].Type, le.Parameters[2].Type, type);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    return Expression.Lambda(delegateType, boxed, le.Parameters);
                }
            }
            else if (expr is ConstantExpression ce)
            {
                if (Constants.IsNull(ce))
                {
                    if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    if (ConstantExpressionHelper.TryGetText(ce, out string text))
                    {
                        Type target = TypeHelper.GetNonNullableType(type);
                        object value = null;

#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = TypeHelper.ParseNumber(text, target);

                                // Make sure an enum value stays an enum value
                                if (target.IsEnum)
                                {
                                    value = Enum.ToObject(target, value);
                                }
                                break;

                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = TypeHelper.ParseNumber(text, target);
                                break;

                            case TypeCode.String:
                                value = TypeHelper.ParseEnum(text, target);
                                break;
                        }
#else
                        if (ce.Type == typeof(Int32) || ce.Type == typeof(UInt32) || ce.Type == typeof(Int64) || ce.Type == typeof(UInt64))
                        {
                            value = TypeHelper.ParseNumber(text, target);

                            // Make sure an enum value stays an enum value
                            if (target.GetTypeInfo().IsEnum)
                            {
                                value = Enum.ToObject(target, value);
                            }
                        }
                        else if (ce.Type == typeof(Double))
                        {
                            if (target == typeof(decimal))
                            {
                                value = TypeHelper.ParseNumber(text, target);
                            }
                        }
                        else if (ce.Type == typeof(String))
                        {
                            value = TypeHelper.ParseEnum(text, target);
                        }
#endif
                        if (value != null)
                        {
                            return Expression.Constant(value, type);
                        }
                    }
                }

                // Try to autopromote string to guid, because in Json and other serializations guid
                // is a value type represented as string.
                if (expr.Type == typeof(string) && type == typeof(Guid))
                {
                    return Expression.Constant(new Guid(Convert.ToString(ce.Value)), typeof(Guid));
                }
            }

            if (TypeHelper.IsCompatibleWith(expr.Type, type, out promotedTarget))
            {
                if (type.GetTypeInfo().IsValueType || exact || expr.Type.GetTypeInfo().IsValueType && convertExpr)
                {
                    return Expression.Convert(expr, promotedTarget ?? type);
                }

                return expr;
            }

            return null;
        }
    }
}
