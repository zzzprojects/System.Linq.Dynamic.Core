namespace System.Linq.Dynamic.Core.Util.Cache;

internal struct CacheEntry<TValue> where TValue : notnull
{
    public TValue Value { get; }

    public DateTime ExpirationTime { get; }

    public CacheEntry(TValue value, DateTime expirationTime) 
    {
        Value = value;
        ExpirationTime = expirationTime;
    }
}