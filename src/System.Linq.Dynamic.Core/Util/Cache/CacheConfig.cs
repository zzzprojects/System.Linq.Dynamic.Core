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
    public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(10);


    /// <summary>
    /// Configures the minimum number of items required in the constant expression cache before triggering cleanup. 
    /// This prevents frequent cleanups, especially in caches with few items. 
    /// A default value of null implies that cleanup is always allowed to run, helping in timely removal of unused cache items.
    /// </summary>
    public int? MinItemsTrigger { get; set; } = null;


    /// <summary>
    /// Sets the frequency for running the cleanup process in the Constant Expression cache. 
    /// By default, cleanup occurs every 10 minutes.
    /// </summary>
    public TimeSpan CleanupFrequency { get; set; } = TimeSpan.FromMinutes(10);
}