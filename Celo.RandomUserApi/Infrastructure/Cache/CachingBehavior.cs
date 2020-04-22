using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.Metrics;
using Celo.RandomUserApi.Infrastructure.Metrics.Types;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Celo.RandomUserApi.Infrastructure.Cache
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IMetricsService _metricsService;

        public CachingBehavior(IMemoryCache memoryCache, IMetricsService metricsService)
        {
            _memoryCache = memoryCache;
            _metricsService = metricsService;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICacheable cacheable)
            {
                // An application could make use of different types of caches:
                // 1. Memory cache that is held in-process for the life time of the application
                // 2. A per request cache - this is a cache that is valid for the duration of a request, following which it is disposed.
                // 3. A distributed cache - an out of process cache like Redis.
                // The ICacheSetting interface provides a common type that represents the different options.
                // I have only provided one implementation for an in memory cache, but this can be extended to cater for above cases if required.
                return cacheable.CacheSetting switch
                {
                    MemCacheSetting memCacheSetting => TryFetchValueFromCache(next, memCacheSetting),
                    _ => next()
                };
            }
            return next();
        }

        private async Task<TResponse> TryFetchValueFromCache(RequestHandlerDelegate<TResponse> next, MemCacheSetting cacheSetting)
        {
            var canHandleUsingCache = _memoryCache.TryGetValue(cacheSetting.CacheKey, out TResponse cachedValue);
            var messageName = typeof(TRequest).Name;
            if (canHandleUsingCache)
            {
                _metricsService.ObserveMetric(
                    new CounterMetric(
                        $"{messageName}_cache_hit_total",
                        $"Total count of cache hits for retrieving {messageName}"));
                return cachedValue;
            }
            var response = await next();
            _memoryCache.Set(cacheSetting.CacheKey, response, cacheSetting.ExpiryPeriod);
            _metricsService.ObserveMetric(
                new CounterMetric(
                    $"{messageName}_cache_miss_total",
                    $"Total count of cache miss for retrieving {messageName}"));
            return response;
        }
    }
}
