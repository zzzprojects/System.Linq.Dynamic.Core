using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// Based on gblog by graeme-hill. https://github.com/graeme-hill/gblog/blob/master/source_content/articles/2014.139_entity-framework-dynamic-queries-and-parameterization.mkd
    /// </summary>
    internal class ConstantExpressionWrapper : IConstantExpressionWrapper
    {
        public void Wrap(ref Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
            {
                if (constantExpression.Type == typeof(bool))
                {
                    expression = WrappedConstant((bool)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(bool?))
                {
                    expression = WrappedConstant((bool?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(char))
                {
                    expression = WrappedConstant((char)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(char?))
                {
                    expression = WrappedConstant((char?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(byte))
                {
                    expression = WrappedConstant((byte)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(byte?))
                {
                    expression = WrappedConstant((byte?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(sbyte))
                {
                    expression = WrappedConstant((sbyte)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(string))
                {
                    expression = WrappedConstant((string)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(float))
                {
                    expression = WrappedConstant((float)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(float?))
                {
                    expression = WrappedConstant((float?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(decimal))
                {
                    expression = WrappedConstant((decimal)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(decimal?))
                {
                    expression = WrappedConstant((decimal?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(double))
                {
                    expression = WrappedConstant((double)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(double?))
                {
                    expression = WrappedConstant((double?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(long))
                {
                    expression = WrappedConstant((long)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(long?))
                {
                    expression = WrappedConstant((long?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(ulong))
                {
                    expression = WrappedConstant((ulong)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(ulong?))
                {
                    expression = WrappedConstant((ulong?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(int))
                {
                    expression = WrappedConstant((int)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(int?))
                {
                    expression = WrappedConstant((int?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(uint))
                {
                    expression = WrappedConstant((uint)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(uint?))
                {
                    expression = WrappedConstant((uint?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(short))
                {
                    expression = WrappedConstant((short)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(short?))
                {
                    expression = WrappedConstant((short?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(ushort))
                {
                    expression = WrappedConstant((ushort)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(ushort?))
                {
                    expression = WrappedConstant((ushort?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(Guid))
                {
                    expression = WrappedConstant((Guid)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(Guid?))
                {
                    expression = WrappedConstant((Guid?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(DateTime))
                {
                    expression = WrappedConstant((DateTime)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(DateTime?))
                {
                    expression = WrappedConstant((DateTime?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(DateTimeOffset))
                {
                    expression = WrappedConstant((DateTimeOffset)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(DateTimeOffset?))
                {
                    expression = WrappedConstant((DateTimeOffset?)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(TimeSpan))
                {
                    expression = WrappedConstant((TimeSpan)constantExpression.Value);
                }
                else if (constantExpression.Type == typeof(TimeSpan?))
                {
                    expression = WrappedConstant((TimeSpan?)constantExpression.Value);
                }

                return;
            }

            if (expression is NewExpression newExpression)
            {
                if (newExpression.Type == typeof(Guid))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<Guid>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(Guid?))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<Guid?>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(DateTime))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<DateTime>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(DateTime?))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<DateTime?>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(DateTimeOffset))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<DateTimeOffset>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(DateTimeOffset?))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<DateTimeOffset?>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(TimeSpan))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<TimeSpan>>(newExpression).Compile()());
                }
                else if (newExpression.Type == typeof(TimeSpan?))
                {
                    expression = WrappedConstant(Expression.Lambda<Func<TimeSpan?>>(newExpression).Compile()());
                }
            }
        }

        private static MemberExpression WrappedConstant<TValue>(TValue value)
        {
            var wrapper = new WrappedValue<TValue>(value);

            return Expression.Property(Expression.Constant(wrapper), typeof(WrappedValue<TValue>).GetProperty("Value"));
        }
    }
}
