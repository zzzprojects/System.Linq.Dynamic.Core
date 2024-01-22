namespace System.Linq.Dynamic.Core.Util.Cache;

internal struct CacheContainer<TValue> where TValue : notnull
{
    public CacheContainer(TValue value, DateTime expirationTime) 
    {
        Value = value;
        ExpirationTime = expirationTime;
    }

    public TValue Value { get; }

    public DateTime ExpirationTime { get; }
}