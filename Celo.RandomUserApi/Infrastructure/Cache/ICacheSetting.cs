namespace Celo.RandomUserApi.Infrastructure.Cache
{
    /// <summary>
    /// Represents the cache settings.
    /// </summary>
    public interface ICacheSetting
    {
        /// <summary>
        /// Gets the Cache key to be used for cache.
        /// </summary>
        string CacheKey { get; }
    }
}
