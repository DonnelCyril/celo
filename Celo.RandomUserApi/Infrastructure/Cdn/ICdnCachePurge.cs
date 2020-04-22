using System;
using System.Threading.Tasks;

namespace Celo.RandomUserApi.Infrastructure.Cdn
{
    /// <summary>
    /// Used to purge the CDN cache.
    /// </summary>
    public interface ICdnCachePurge
    {

        /// <summary>
        /// Purges individual user records cached at CDN.
        /// </summary>
        /// <remarks>
        /// This is invoked when a user record has been updated.
        /// In this scenario we only want the individual user record to be cached from the CDN. 
        /// Responses returned by the 
        /// </remarks>
        Task PurgeUserRecord(Guid userId);

        /// <summary>
        /// Purges pages that has reference to the provided user record.
        /// </summary>
        /// <remarks>
        /// This is invoked when a user record is deleted. In this scenario we want the page containing references to this record to be purged.
        /// </remarks>
        Task PurgePagesWithUser(Guid userId);
    }
}