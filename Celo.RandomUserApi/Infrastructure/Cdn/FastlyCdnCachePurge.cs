using System;
using System.Threading.Tasks;

namespace Celo.RandomUserApi.Infrastructure.Cdn
{
    public class FastlyCdnCachePurge : ICdnCachePurge
    {
        // Full implementation not provided here due to time constraints and would require setting up a service in Fastly.
        // The implementation itself is simple and is described below.
        public Task PurgeUserRecord(Guid userId)
        {
            // This will be PURGE request to the user record url in CDN.
            return Task.CompletedTask;
        }

        /// <remarks>
        /// We rely on the surrogate-keys feature in Fastly CDN.
        /// userId is used to form a surrogate key when returning UserIndex pages. <see cref="CacheHeaders.GetUserIndexPageCacheHeaders"/>
        /// This link explains surrogate keys in detail: https://docs.fastly.com/en/guides/purging-api-cache-with-surrogate-keys
        /// This allows us to purge all pages that has a reference to the provided user id. 
        /// </remarks>
        public Task PurgePagesWithUser(Guid userId)
        {
            // This will be a purge request to a dedicated a purge endpoint.
            // PURGE /service/:service_id/userId
            return Task.CompletedTask;
        }
    }
}