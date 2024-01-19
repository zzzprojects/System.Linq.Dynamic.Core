using System.Collections.Concurrent;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class ThreadSafeSlidingCache<TKey, TValue> where TKey : notnull where TValue : notnull
    {
        private readonly ConcurrentDictionary<TKey, (TValue Value, DateTime ExpirationTime)> _cache;
        private readonly TimeSpan _timeToLive;
        private readonly TimeSpan _cleanupFrequency;
        private DateTime _lastCleanupTime = DateTime.MinValue;

        public ThreadSafeSlidingCache(TimeSpan timeToLive, TimeSpan? cleanupFrequency = null)
        {
            _cache = new ConcurrentDictionary<TKey, (TValue, DateTime)>();
            _timeToLive = timeToLive;
            _cleanupFrequency = cleanupFrequency ?? TimeSpan.FromSeconds(10);
        }

        public TimeSpan TimeToLive => _timeToLive;

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var expirationTime = DateTime.UtcNow.Add(_timeToLive);
            _cache[key] = (value, expirationTime);

            CleanupIfNeeded();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            CleanupIfNeeded();

            if (_cache.TryGetValue(key, out var valueAndExpiration))
            {
                if (DateTime.UtcNow <= valueAndExpiration.ExpirationTime)
                {
                    value = valueAndExpiration.Value;
                    _cache[key] = (value, DateTime.UtcNow.Add(_timeToLive));
                    return true;
                }
                else
                {
                    // Remove expired item
                    _cache.TryRemove(key, out _);
                }
            }

            value = default!;
            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var removed = _cache.TryRemove(key, out _);
            CleanupIfNeeded();
            return removed;
        }

        private void CleanupIfNeeded()
        {
            if (DateTime.UtcNow - _lastCleanupTime > _cleanupFrequency)
            {
                foreach (var key in _cache.Keys)
                {
                    if (DateTime.UtcNow > _cache[key].ExpirationTime)
                    {
                        _cache.TryRemove(key, out _);
                    }
                }
                _lastCleanupTime = DateTime.UtcNow;
            }
        }
    }
}
