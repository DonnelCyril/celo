namespace Celo.RandomUserApi.Infrastructure.Cache
{
    /// <summary>
    /// A request which can be cached.
    /// </summary>
    public interface ICacheable
    {
        ICacheSetting CacheSetting { get; }
    }
}
