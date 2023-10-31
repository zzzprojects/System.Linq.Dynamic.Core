#if !NET35
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Linq.Dynamic.Core;

public class DynamicTuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable
{
    private readonly T1? _item1;
    private readonly T2? _item2;

    public T1 Item1 => _item1!;
    public T2 Item2 => _item2!;

    public DynamicTuple()
    {
        _item1 = default;
        _item2 = default;
    }

    public DynamicTuple(T1 item1, T2 item2)
    {
        _item1 = item1;
        _item2 = item2;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals(obj, EqualityComparer<object>.Default);
    }

    bool IStructuralEquatable.Equals([NotNullWhen(true)] object? other, IEqualityComparer comparer)
    {
        return Equals(other, comparer);
    }

    private bool Equals([NotNullWhen(true)] object? other, IEqualityComparer comparer)
    {
        if (other is not DynamicTuple<T1, T2> objTuple)
        {
            return false;
        }

        return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2);
    }

    int IComparable.CompareTo(object? obj)
    {
        return CompareTo(obj, Comparer<object>.Default);
    }

    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        return CompareTo(other, comparer);
    }

    private int CompareTo(object? other, IComparer comparer)
    {
        if (other == null)
        {
            return 1;
        }

        if (other is not DynamicTuple<T1, T2> objTuple)
        {
            throw new ArgumentException($"Argument must be of type {GetType()}", nameof(other));
        }

        var c = comparer.Compare(_item1, objTuple._item1);

        return c != 0 ? c : comparer.Compare(_item2, objTuple._item2);
    }

    public override int GetHashCode()
    {
        return GetHashCode(EqualityComparer<object>.Default);
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        return GetHashCode(comparer);
    }

    private int GetHashCode(IEqualityComparer comparer)
    {
        return CombineHashCodes(comparer.GetHashCode(_item1!), comparer.GetHashCode(_item2!));
    }

    private static int CombineHashCodes(int h1, int h2)
    {
        return ((h1 << 5) + h1) ^ h2;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('(');
        return ToString(sb);
    }

    private string ToString(StringBuilder sb)
    {
        sb.Append(_item1);
        sb.Append(", ");
        sb.Append(_item2);
        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// The number of positions in this data structure.
    /// </summary>
    public int Length => 2;

    /// <summary>
    /// Get the element at position <param name="index"/>.
    /// </summary>
    object? this[int index] =>
        index switch
        {
            0 => Item1,
            1 => Item2,
            _ => throw new IndexOutOfRangeException(),
        };
}
#endif