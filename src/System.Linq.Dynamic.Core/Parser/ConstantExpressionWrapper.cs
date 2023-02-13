using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
#if UAP10_0 || NETSTANDARD1_3
using System.Reflection;
#endif

namespace System.Linq.Dynamic.Core.Parser;

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
                expression = Wrap((bool)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(bool?))
            {
                expression = Wrap((bool?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(char))
            {
                expression = Wrap((char)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(char?))
            {
                expression = Wrap((char?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(byte))
            {
                expression = Wrap((byte)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(byte?))
            {
                expression = Wrap((byte?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(sbyte))
            {
                expression = Wrap((sbyte)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(string))
            {
                expression = Wrap((string)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(float))
            {
                expression = Wrap((float)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(float?))
            {
                expression = Wrap((float?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(decimal))
            {
                expression = Wrap((decimal)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(decimal?))
            {
                expression = Wrap((decimal?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(double))
            {
                expression = Wrap((double)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(double?))
            {
                expression = Wrap((double?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(long))
            {
                expression = Wrap((long)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(long?))
            {
                expression = Wrap((long?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(ulong))
            {
                expression = Wrap((ulong)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(ulong?))
            {
                expression = Wrap((ulong?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(int))
            {
                expression = Wrap((int)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(int?))
            {
                expression = Wrap((int?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(uint))
            {
                expression = Wrap((uint)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(uint?))
            {
                expression = Wrap((uint?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(short))
            {
                expression = Wrap((short)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(short?))
            {
                expression = Wrap((short?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(ushort))
            {
                expression = Wrap((ushort)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(ushort?))
            {
                expression = Wrap((ushort?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(Guid))
            {
                expression = Wrap((Guid)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(Guid?))
            {
                expression = Wrap((Guid?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(DateTime))
            {
                expression = Wrap((DateTime)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(DateTime?))
            {
                expression = Wrap((DateTime?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(DateTimeOffset))
            {
                expression = Wrap((DateTimeOffset)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(DateTimeOffset?))
            {
                expression = Wrap((DateTimeOffset?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(TimeSpan))
            {
                expression = Wrap((TimeSpan)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(TimeSpan?))
            {
                expression = Wrap((TimeSpan?)constantExpression.Value);
            }
#if NET6_0_OR_GREATER
            else if (constantExpression.Type == typeof(DateOnly))
            {
                expression = Wrap((DateOnly)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(DateOnly?))
            {
                expression = Wrap((DateOnly?)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(TimeOnly))
            {
                expression = Wrap((TimeOnly)constantExpression.Value);
            }
            else if (constantExpression.Type == typeof(TimeOnly?))
            {
                expression = Wrap((TimeOnly?)constantExpression.Value);
            }
#endif
            return;
        }

        if (expression is NewExpression newExpression)
        {
            if (newExpression.Type == typeof(Guid))
            {
                expression = Wrap(Expression.Lambda<Func<Guid>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(Guid?))
            {
                expression = Wrap(Expression.Lambda<Func<Guid?>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(DateTime))
            {
                expression = Wrap(Expression.Lambda<Func<DateTime>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(DateTime?))
            {
                expression = Wrap(Expression.Lambda<Func<DateTime?>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(DateTimeOffset))
            {
                expression = Wrap(Expression.Lambda<Func<DateTimeOffset>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(DateTimeOffset?))
            {
                expression = Wrap(Expression.Lambda<Func<DateTimeOffset?>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(TimeSpan))
            {
                expression = Wrap(Expression.Lambda<Func<TimeSpan>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(TimeSpan?))
            {
                expression = Wrap(Expression.Lambda<Func<TimeSpan?>>(newExpression).Compile()());
            }
#if NET6_0_OR_GREATER
            else if (newExpression.Type == typeof(DateOnly))
            {
                expression = Wrap(Expression.Lambda<Func<DateOnly>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(DateOnly?))
            {
                expression = Wrap(Expression.Lambda<Func<DateOnly?>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(TimeOnly))
            {
                expression = Wrap(Expression.Lambda<Func<TimeOnly>>(newExpression).Compile()());
            }
            else if (newExpression.Type == typeof(TimeOnly?))
            {
                expression = Wrap(Expression.Lambda<Func<TimeOnly?>>(newExpression).Compile()());
            }
#endif
        }
    }

    public bool TryUnwrapAsValue<TValue>(MemberExpression? expression, [NotNullWhen(true)] out TValue? value)
    {
        if (expression?.Expression is ConstantExpression { Value: WrappedValue<TValue> wrapper })
        {
            value = wrapper.Value!;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryUnwrapAsConstantExpression<TValue>(MemberExpression? expression, [NotNullWhen(true)] out ConstantExpression? value)
    {
        if (TryUnwrapAsValue<TValue>(expression, out var wrappedValue))
        {
            value = Expression.Constant(wrappedValue);
            return true;
        }

        value = default;
        return false;
    }

    private static MemberExpression Wrap<TValue>(TValue value)
    {
        var wrapper = new WrappedValue<TValue>(value);

        return Expression.Property(Expression.Constant(wrapper), typeof(WrappedValue<TValue>).GetProperty("Value")!);
    }
}