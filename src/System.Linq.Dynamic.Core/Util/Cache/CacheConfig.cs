namespace System.Linq.Dynamic.Core.Util.Cache;

/// <summary>
/// Cache Configuration Options
/// </summary>
public class CacheConfig
{
    /// <summary>
    /// Sets a Time-To-Live (TTL) for items in the constant expression cache to prevent uncontrolled growth. 
    /// Items not accessed within this TTL will be expired, allowing garbage collection to reclaim the memory.
    /// Default is 10 minutes.
    /// </summary>
    public TimeSpan TimeToLive { get; set; } = SlidingCacheConstants.DefaultTimeToLive;

    /// <summary>
    /// Configures the minimum number of items required in the constant expression cache before triggering cleanup. 
    /// This prevents frequent cleanups, especially in caches with few items. 
    /// A default value of null implies that cleanup is always allowed to run, helping in timely removal of unused cache items.
    /// </summary>
    public int? MinItemsTrigger { get; set; }

    /// <summary>
    /// Sets the frequency for running the cleanup process in the Constant Expression cache. 
    /// By default, cleanup occurs every 10 minutes.
    /// </summary>
    public TimeSpan CleanupFrequency { get; set; } = SlidingCacheConstants.DefaultCleanupFrequency;

    /// <summary>
    /// Enables returning expired cache items in scenarios where cleanup, running on a separate thread, 
    /// has not yet removed them. This allows for the retrieval of an expired object without needing to 
    /// clear and recreate it if a request is made concurrently with cleanup. Particularly useful 
    /// when cached items are deterministic, ensuring consistent results even from expired entries.
    /// Default true;
    /// </summary>
    public bool ReturnExpiredItems { get; set; } = true;
}