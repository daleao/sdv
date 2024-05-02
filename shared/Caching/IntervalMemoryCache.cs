namespace DaLion.Shared.Caching;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>A memory cache with sliding expiry based on custom intervals, with no background processing.</summary>
/// <typeparam name="TKey">The cache key type.</typeparam>
/// <typeparam name="TValue">The cache value type.</typeparam>
/// <remarks>
///        This is optimized for small caches that are reset relatively rarely. Each cache entry is marked as hot (accessed since the interval started) or stale.
///        When a new interval is started, stale entries are cleared and hot entries become stale.
///        Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.
/// </remarks>
internal sealed class IntervalMemoryCache<TKey, TValue>
    where TKey : notnull
{
    /// <summary>The cached values that were accessed during the current interval.</summary>
    private Dictionary<TKey, TValue> _hotCache = new();

    /// <summary>The cached values that will expire on the next interval.</summary>
    private Dictionary<TKey, TValue> _staleCache = new();

    /// <summary>Gets a value from the cache, fetching it first if needed.</summary>
    /// <param name="cacheKey">The unique key for the cached value.</param>
    /// <param name="get">A delegate for getting the latest data if it's not in the cache yet.</param>
    /// <returns>The cached value corresponding to the given <paramref name="cacheKey"/>.</returns>
    public TValue GetOrSet(TKey cacheKey, Func<TValue> get)
    {
        if (this._hotCache.TryGetValue(cacheKey, out var value))
        {
            return value;
        }

        if (this._staleCache.TryGetValue(cacheKey, out value))
        {
            this._hotCache[cacheKey] = value;
            return value;
        }

        value = get();
        this._hotCache[cacheKey] = value;
        return value;
    }

    /// <summary>Starts a new cache interval, removing any stale entries.</summary>
    public void StartNewInterval()
    {
        this._staleCache.Clear();
        if (this._hotCache.Count == 0)
        {
            return;
        }

        (this._hotCache, this._staleCache) = (this._staleCache, this._hotCache);
    }
}
