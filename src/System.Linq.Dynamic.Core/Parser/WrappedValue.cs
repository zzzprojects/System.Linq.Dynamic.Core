using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Parser;

internal class WrappedValue<TValue>
{
    public TValue Value { get; }

    public WrappedValue(TValue value)
    {
        Value = value;
    }

    public static bool operator ==(WrappedValue<TValue>? left, WrappedValue<TValue>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return EqualityComparer<TValue>.Default.Equals(left.Value, right.Value);
    }

    public static bool operator !=(WrappedValue<TValue>? left, WrappedValue<TValue>? right)
    {
        return !(left == right);
    }

    public static bool operator ==(WrappedValue<TValue>? left, TValue? right)
    {
        if (ReferenceEquals(left, null))
        {
            return false;
        }

        return EqualityComparer<TValue>.Default.Equals(left.Value, right);
    }

    public static bool operator !=(WrappedValue<TValue>? left, TValue? right)
    {
        return !(left == right);
    }

    public static bool operator ==(TValue? left, WrappedValue<TValue>? right)
    {
        if (ReferenceEquals(right, null))
        {
            return false;
        }

        return EqualityComparer<TValue>.Default.Equals(left, right.Value);
    }

    public static bool operator !=(TValue? left, WrappedValue<TValue>? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not WrappedValue<TValue> other)
        {
            return false;
        }

        return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }
}