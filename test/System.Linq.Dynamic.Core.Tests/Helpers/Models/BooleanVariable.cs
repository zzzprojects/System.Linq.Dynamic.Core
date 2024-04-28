namespace System.Linq.Dynamic.Core.Tests.Helpers.Models;

public readonly struct BooleanVariable : IEquatable<bool>, IEquatable<BooleanVariable>, IConvertible
{
    private readonly bool _innerValue;

    public BooleanVariable(bool innerValue)
    {
        _innerValue = innerValue;
    }

    public override bool Equals(object obj)
    {
        return obj is bool val && Equals(val);
    }

    public bool Equals(bool other)
    {
        return _innerValue == other;
    }

    public bool Equals(BooleanVariable other)
    {
        return Equals(other._innerValue);
    }

    public override int GetHashCode()
    {
        return _innerValue.GetHashCode();
    }

    public static implicit operator bool(BooleanVariable v) => v._innerValue;

    public static implicit operator BooleanVariable(bool b) => new BooleanVariable(b);

    public static bool operator true(BooleanVariable v) => v._innerValue;

    public static bool operator false(BooleanVariable v) => !v._innerValue;

    // comparison to bool
    public static bool operator ==(BooleanVariable a, bool b) => a._innerValue == b;

    public static bool operator !=(BooleanVariable a, bool b) => a._innerValue != b;

    // comparison to int
    public static bool operator ==(BooleanVariable a, int b) => a._innerValue == (b != 0);

    public static bool operator !=(BooleanVariable a, int b) => a._innerValue != (b != 0);

    // comparison to decimal
    public static bool operator ==(BooleanVariable a, decimal b) => a._innerValue == (b != 0);

    public static bool operator !=(BooleanVariable a, decimal b) => a._innerValue != (b != 0);

    // To/from self
    public static bool operator ==(BooleanVariable a, BooleanVariable b) => a._innerValue == b._innerValue;

    public static bool operator !=(BooleanVariable a, BooleanVariable b) => a._innerValue != b._innerValue;

    #region IConvertible

    public TypeCode GetTypeCode()
    {
        return _innerValue.GetTypeCode();
    }

    public bool ToBoolean(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToBoolean(provider);
    }

    public byte ToByte(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToByte(provider);
    }

    public char ToChar(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToChar(provider);
    }

    public DateTime ToDateTime(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToDateTime(provider);
    }

    public decimal ToDecimal(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToDecimal(provider);
    }

    public double ToDouble(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToDouble(provider);
    }

    public short ToInt16(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToInt16(provider);
    }

    public int ToInt32(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToInt32(provider);
    }

    public long ToInt64(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToInt64(provider);
    }

    public sbyte ToSByte(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToSByte(provider);
    }

    public float ToSingle(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToSingle(provider);
    }

    public string ToString(IFormatProvider provider)
    {
        return _innerValue.ToString(provider);
    }

    public object ToType(Type conversionType, IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToType(conversionType, provider);
    }

    public ushort ToUInt16(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToUInt16(provider);
    }

    public uint ToUInt32(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToUInt32(provider);
    }

    public ulong ToUInt64(IFormatProvider provider)
    {
        return ((IConvertible)_innerValue).ToUInt64(provider);
    }
    #endregion
}