namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public struct SnowflakeId : IEquatable<SnowflakeId>
    {
        public bool Equals(SnowflakeId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is SnowflakeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(SnowflakeId left, SnowflakeId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SnowflakeId left, SnowflakeId right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(SnowflakeId left, int right)
        {
            return (int)left.Value == right;
        }

        public static bool operator !=(SnowflakeId left, int right)
        {
            return (int)left.Value != right;
        }

        public static bool operator ==(SnowflakeId left, ulong right)
        {
            return left.Value == right;
        }

        public static bool operator !=(SnowflakeId left, ulong right)
        {
            return left.Value != right;
        }

        public ulong Value { get; }

        public SnowflakeId(ulong value)
        {
            Value = value;
        }
    }
}
