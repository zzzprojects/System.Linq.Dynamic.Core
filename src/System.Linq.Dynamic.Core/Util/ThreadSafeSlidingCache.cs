using System.Collections.Concurrent;
using System.Linq.Dynamic.Core.Validation;
using System.Threading.Tasks;

namespace System.Linq.Dynamic.Core.Util
{
    internal static class ThreadSafeSlidingCacheConstants
    {
        // Default cleanup frequency
        public static readonly TimeSpan DefaultCleanupFrequency = TimeSpan.FromMinutes(10);
    }

    internal class ThreadSafeSlidingCache<TKey, TValue> where TKey : notnull where TValue : notnull
    {
        private readonly ConcurrentDictionary<TKey, (TValue Value, DateTime ExpirationTime)> _cache;
        private readonly TimeSpan _cleanupFrequency;
        private readonly IDateTimeUtils _dateTimeProvider;
        private readonly Func<Task> _deleteExpiredCachedItemsDelegate;
        private readonly long? _minCacheItemsBeforeCleanup;
        private DateTime _lastCleanupTime = DateTime.MinValue;

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
            _cache = new ConcurrentDictionary<TKey, (TValue, DateTime)>();
            TimeToLive = timeToLive;
            _minCacheItemsBeforeCleanup = minCacheItemsBeforeCleanup;
            _cleanupFrequency = cleanupFrequency ?? ThreadSafeSlidingCacheConstants.DefaultCleanupFrequency;
            _deleteExpiredCachedItemsDelegate = Cleanup;
            _dateTimeProvider = dateTimeProvider ?? new DateTimeUtils();
        }

        public TimeSpan TimeToLive { get; }

        /// <summary>
        ///     Provide the number of items in the cache
        /// </summary>
        public int Count => _cache.Count;

        public void AddOrUpdate(TKey key, TValue value)
        {
            Check.NotNull(key);
            Check.NotNull(value);

            var expirationTime = _dateTimeProvider.UtcNow.Add(TimeToLive);
            _cache[key] = (value, expirationTime);

            CleanupIfNeeded();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Check.NotNull(key);

            CleanupIfNeeded();

            if (_cache.TryGetValue(key, out var valueAndExpiration))
            {
                if (_dateTimeProvider.UtcNow <= valueAndExpiration.ExpirationTime)
                {
                    value = valueAndExpiration.Value;
                    _cache[key] = (value, _dateTimeProvider.UtcNow.Add(TimeToLive));
                    return true;
                }

                // Remove expired item
                _cache.TryRemove(key, out _);
            }

            value = default!;
            return false;
        }

        public bool Remove(TKey key)
        {
            Check.NotNull(key);

            var removed = _cache.TryRemove(key, out _);
            CleanupIfNeeded();
            return removed;
        }

        /// <summary>
        ///     Check if cache needs to be cleaned up.
        ///     If it does, span the cleanup as a Task to prevent from blocking
        /// </summary>
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

                Task.Run(_deleteExpiredCachedItemsDelegate);
            }
        }

        /// <summary>
        ///     Cleanup the Cache
        /// </summary>
        /// <returns></returns>
        private Task Cleanup()
        {
            foreach (var key in _cache.Keys)
            {
                if (_dateTimeProvider.UtcNow > _cache[key].ExpirationTime)
                {
                    _cache.TryRemove(key, out _);
                }
            }

            return Task.CompletedTask;
        }
    }
}