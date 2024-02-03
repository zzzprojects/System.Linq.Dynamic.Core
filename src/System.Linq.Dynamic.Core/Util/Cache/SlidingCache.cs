using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;
using System.Threading;

namespace System.Linq.Dynamic.Core.Util.Cache;

internal class SlidingCache<TKey, TValue> where TKey : notnull where TValue : notnull
{
    private readonly ConcurrentDictionary<TKey, CacheEntry<TValue>> _cache;
    private readonly TimeSpan _cleanupFrequency;
    private readonly IDateTimeUtils _dateTimeProvider;
    private readonly Action _deleteExpiredCachedItemsDelegate;
    private readonly long? _minCacheItemsBeforeCleanup;
    private readonly bool _returnExpiredItems;
    private DateTime _lastCleanupTime;
    private int _cleanupLocked = 0;

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
    /// <param name="returnExpiredItems">If a request for an item happens to be expired, but is still
    /// in known, don't expire it and return it instead.</param>
    public SlidingCache(
        TimeSpan timeToLive,
        TimeSpan? cleanupFrequency = null,
        long? minCacheItemsBeforeCleanup = null,
        IDateTimeUtils? dateTimeProvider = null,
        bool returnExpiredItems = false)
    {
        _cache = new ConcurrentDictionary<TKey, CacheEntry<TValue>>();
        TimeToLive = timeToLive;
        _minCacheItemsBeforeCleanup = minCacheItemsBeforeCleanup;
        _returnExpiredItems = returnExpiredItems;
        _cleanupFrequency = cleanupFrequency ?? SlidingCacheConstants.DefaultCleanupFrequency;
        _deleteExpiredCachedItemsDelegate = Cleanup;
        _dateTimeProvider = dateTimeProvider ?? new DateTimeUtils();
        // To prevent a scan on first call, set the last Cleanup to the current Provider time
        _lastCleanupTime = _dateTimeProvider.UtcNow;
    }

    /// <summary>
    /// Sliding Thread Safe Cache
    /// </summary>
    /// <param name="cacheConfig">The <see cref="CacheConfig"/> to use.</param>
    /// <param name="dateTimeProvider">
    ///     Provides the Time for the Caching object. Default will be created if not supplied. Used
    ///     for Testing classes
    /// </param>
    public SlidingCache(CacheConfig cacheConfig, IDateTimeUtils? dateTimeProvider = null)
    {
        Check.NotNull(cacheConfig);

        _cache = new ConcurrentDictionary<TKey, CacheEntry<TValue>>();
        TimeToLive = cacheConfig.TimeToLive;
        _minCacheItemsBeforeCleanup = cacheConfig.MinItemsTrigger;
        _cleanupFrequency = cacheConfig.CleanupFrequency;
        _returnExpiredItems = cacheConfig.ReturnExpiredItems;
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
        _cache[key] = new CacheEntry<TValue>(value, expirationTime);

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

        try
        {
            if (_cache.TryGetValue(key, out var valueAndExpiration))
            {
                // Permit expired returns will return the object even if was expired
                // this will prevent the need to re-create the object for the caller
                if (_returnExpiredItems || _dateTimeProvider.UtcNow <= valueAndExpiration.ExpirationTime)
                {
                    value = valueAndExpiration.Value;
                    var newExpire = _dateTimeProvider.UtcNow.Add(TimeToLive);
                    _cache[key] = new CacheEntry<TValue>(value, newExpire);
                    return true;
                }

                // Remove expired item
                _cache.TryRemove(key, out _);
            }
        }
        finally
        {
            // If permit expired returns are enabled,
            // we want to ensure the cache has a chance to get the value
            CleanupIfNeeded();
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
        // Ensure this is only executing one at a time.
        if (Interlocked.CompareExchange(ref _cleanupLocked, 1, 0) != 0)
        {
            return;
        }

        try
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
        finally
        {
            // Release the lock
            _cleanupLocked = 0;
        }
    }

    private void Cleanup()
    {
        // Enumerate the key/value - safe per docs
        foreach (var keyValue in _cache)
        {
            if (_dateTimeProvider.UtcNow > keyValue.Value.ExpirationTime)
            {
                _cache.TryRemove(keyValue.Key, out _);
            }
        }
    }
}