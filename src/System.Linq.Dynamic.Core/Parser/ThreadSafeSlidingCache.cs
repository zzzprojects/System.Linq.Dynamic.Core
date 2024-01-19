using System.Collections.Concurrent;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class ThreadSafeSlidingCache<T1, T2> where T1 : notnull where T2 : notnull
    {
        private readonly ConcurrentDictionary<T1, (T2 Value, DateTime ExpirationTime)> _cache;
        private readonly TimeSpan _timeToLive;
        private readonly TimeSpan _cleanupFrequency;
        private DateTime _lastCleanupTime = DateTime.MinValue;

        public ThreadSafeSlidingCache(TimeSpan timeToLive, TimeSpan? cleanupFrequency = null)
        {
            _cache = new ConcurrentDictionary<T1, (T2, DateTime)>();
            _timeToLive = timeToLive;
            _cleanupFrequency = cleanupFrequency ?? TimeSpan.FromSeconds(10);
        }

        public TimeSpan TimeToLive => _timeToLive;

        public void AddOrUpdate(T1 key, T2 value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var expirationTime = DateTime.UtcNow.Add(_timeToLive);
            _cache[key] = (value, expirationTime);

            CleanupIfNeeded();
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            CleanupIfNeeded();

            if (_cache.TryGetValue(key, out var valueAndExpiration))
            {
                if (DateTime.UtcNow <= valueAndExpiration.ExpirationTime)
                {
                    value = valueAndExpiration.Value;
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

        public bool Remove(T1 key)
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
