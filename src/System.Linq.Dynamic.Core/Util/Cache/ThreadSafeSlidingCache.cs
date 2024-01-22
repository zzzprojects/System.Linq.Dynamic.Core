using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.Util.Cache;

internal class ThreadSafeSlidingCache<TKey, TValue> where TKey : notnull where TValue : notnull
{
    private readonly ConcurrentDictionary<TKey, CacheContainer<TValue>> _cache;
    private readonly TimeSpan _cleanupFrequency;
    private readonly IDateTimeUtils _dateTimeProvider;
    private readonly Action _deleteExpiredCachedItemsDelegate;
    private readonly long? _minCacheItemsBeforeCleanup;
    private DateTime _lastCleanupTime;

    /// <summary>
    ///     Sliding Thread Safe Cache
    /// </summary>
    /// <param name="timeToLive">The length of time any object would survive before being removed</param>
    /// <param name="cleanupFrequency">Only look for expired objects over specific periods</param>
    /// <param name="minCacheItemsBeforeCleanup">
    ///     If defined, only allow the cleanup process after x number of cached items have
    ///     been stored
    /// </param>
    /// <param name="dateTimeProvider">
    ///     Provides the Time for the Caching object. Default will be created if not supplied. Used
    ///     for Testing classes
    /// </param>
    public ThreadSafeSlidingCache(
        TimeSpan timeToLive,
        TimeSpan? cleanupFrequency = null,
        long? minCacheItemsBeforeCleanup = null,
        IDateTimeUtils? dateTimeProvider = null)
    {
        _cache = new ConcurrentDictionary<TKey, CacheContainer<TValue>>();
        TimeToLive = timeToLive;
        _minCacheItemsBeforeCleanup = minCacheItemsBeforeCleanup;
        _cleanupFrequency = cleanupFrequency ?? ThreadSafeSlidingCacheConstants.DefaultCleanupFrequency;
        _deleteExpiredCachedItemsDelegate = Cleanup;
        _dateTimeProvider = dateTimeProvider ?? new DateTimeUtils();
        // To prevent a scan on first call, set the last Cleanup to the current Provider time
        _lastCleanupTime = _dateTimeProvider.UtcNow;
    }

    /// <summary>
    /// Cache TTL value
    /// </summary>
    public TimeSpan TimeToLive { get; }

    /// <summary>
    ///     Provide the number of items in the cache
    /// </summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Add or update the item in the cache, at the same time update the expiration time
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddOrUpdate(TKey key, TValue value)
    {
        Check.NotNull(key);
        Check.NotNull(value);

        var expirationTime = _dateTimeProvider.UtcNow.Add(TimeToLive);
        _cache[key] = new CacheContainer<TValue>(value, expirationTime);

        CleanupIfNeeded();
    }

    /// <summary>
    /// Attempt to get the value from the cache. This will extend the cache expiration time if the item is found
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        Check.NotNull(key);

        CleanupIfNeeded();

        if (_cache.TryGetValue(key, out var valueAndExpiration))
        {
            if (_dateTimeProvider.UtcNow <= valueAndExpiration.ExpirationTime)
            {
                value = valueAndExpiration.Value;
                var newExpire = _dateTimeProvider.UtcNow.Add(TimeToLive);
                _cache[key] = new CacheContainer<TValue>(value, newExpire);
                return true;
            }

            // Remove expired item
            _cache.TryRemove(key, out _);
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        Check.NotNull(key);

        var removed = _cache.TryRemove(key, out _);
        CleanupIfNeeded();
        return removed;
    }

    private void CleanupIfNeeded()
    {
        if (_dateTimeProvider.UtcNow - _lastCleanupTime > _cleanupFrequency
            && (_minCacheItemsBeforeCleanup == null ||
                _cache.Count >=
                _minCacheItemsBeforeCleanup) // Only cleanup if we have a minimum number of items in the cache.
           )
        {
            // Set here, so we don't have re-entry due to large collection enumeration.
            _lastCleanupTime = _dateTimeProvider.UtcNow;

            TaskUtils.Run(_deleteExpiredCachedItemsDelegate);
        }
    }

    private void Cleanup()
    {
        foreach (var key in _cache.Keys)
        {
            if (_dateTimeProvider.UtcNow > _cache[key].ExpirationTime)
            {
                _cache.TryRemove(key, out _);
            }
        }
    }
}
