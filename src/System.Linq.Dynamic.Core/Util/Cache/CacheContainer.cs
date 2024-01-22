namespace System.Linq.Dynamic.Core.Util.Cache;

internal struct CacheContainer<TValue> where TValue : notnull
{
    public TValue Value { get; }

    public DateTime ExpirationTime { get; }

    public CacheContainer(TValue value, DateTime expirationTime) 
    {
        Value = value;
        ExpirationTime = expirationTime;
    }
}