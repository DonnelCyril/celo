using System;

namespace Celo.RandomUserApi.Infrastructure.Cache
{
    public class MemCacheSetting : ICacheSetting
    {
        public MemCacheSetting(string cacheKey, TimeSpan expiryPeriod)
        {
            CacheKey = cacheKey;
            ExpiryPeriod = expiryPeriod;
        }

        public TimeSpan ExpiryPeriod { get; }

        public string CacheKey { get; }

    }
}
