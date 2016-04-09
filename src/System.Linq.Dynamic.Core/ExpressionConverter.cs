#if NETFX
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Added for WinRT
    /// </summary>
    public static class ExpressionConverter
    {
        private static readonly ExpressionConverterVisitor visitor = new ExpressionConverterVisitor();

        public static Expression DynamicObjectClassToAnonymousType(this Expression expression)
        {
            return visitor.Visit(expression);
        }

        private class ExpressionConverterVisitor : ExpressionVisitor
        {
            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                if (node.Body is NewExpression && ((NewExpression)node.Body).Type == typeof(DynamicObjectClass))
                {
                    var e = node.Body as NewExpression;

                    var properties = new List<DynamicProperty>(e.Arguments.Count);
                    var expressions = new List<Expression>(e.Arguments.Count);
                    foreach (NewExpression newEx in e.Arguments)
                    {
                        var name = ((ConstantExpression)newEx.Arguments.First()).Value as string;
                        var parameter = ((UnaryExpression)newEx.Arguments.Skip(1).First()).Operand;
                        properties.Add(new DynamicProperty(name, parameter.Type));
                        expressions.Add(parameter);
                    }

                    Type type = DynamicExpression.CreateClass(properties);

                    MemberBinding[] bindings = new MemberBinding[properties.Count];
                    for (int i = 0; i < bindings.Length; i++)
                        bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
                    var membInit = Expression.MemberInit(Expression.New(type), bindings);

                    var typeOfTArgs = typeof(T).GetGenericArguments();

                    var funcTType = typeof(Func<,>).MakeGenericType(new[] { typeOfTArgs.First(), type });
                    var mi = typeof(Expression).GetMethods().FirstOrDefault(x => x.Name == "Lambda" && x.ContainsGenericParameters);
                    MethodInfo genericMethod = mi.MakeGenericMethod(new[] { funcTType });
                    var lambda = genericMethod.Invoke(null, new object[] { membInit, node.Parameters.ToArray() }) as Expression;
                    return lambda;
                }
                return base.VisitLambda<T>(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == "Select")
                {
                    var arguments = node.Arguments.ToList();
                    for (int n = 0; n < arguments.Count; n++)
                        arguments[n] = visitor.Visit(arguments[n]);
                    var typeList = arguments.Select(x => x.Type).ToArray();
                    var funcTType = typeof(Func<,>).MakeGenericType(typeList);

                    var argsmth = node.Method.GetGenericArguments().ToArray();
                    argsmth[1] = ((LambdaExpression)((UnaryExpression)arguments[1]).Operand).Body.Type;
                    var mi = node.Method.DeclaringType.GetMethods().FirstOrDefault(x => x.Name == "Select");
                    var mth = mi.MakeGenericMethod(argsmth);

                    return Expression.Call(mth, arguments);
                }
                return base.VisitMethodCall(node);
            }
        }
    }
}
#endif