using System.Collections.Generic;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using Microsoft.Net.Http.Headers;

namespace Celo.RandomUserApi.Infrastructure.Cdn
{
    public static class CacheHeaders
    {
        public static IDictionary<string, string> GetEtagHeader(string etag)
        {
            etag.ThrowIfNullOrEmpty(nameof(etag));
            return new Dictionary<string, string> {{HeaderNames.ETag, etag}};
        }

        public static IDictionary<string, string> GetUserIndexPageCacheHeaders(UserIndexDto userIndexDto)
        {
            userIndexDto.ThrowIfNull(nameof(userIndexDto));
            var headers = CacheAtCdnFor(GetSecondsInAYear());
            headers.Add("Surrogate-Key", string.Join(" ", userIndexDto.UserIds));
            return headers;
        }

        public static IDictionary<string, string> GetUserCacheHeaders(UserDto userDto, string etag)
        {
            userDto.ThrowIfNull(nameof(userDto));
            etag.ThrowIfNullOrEmpty(nameof(etag));
            var headers = CacheAtCdnFor(GetSecondsInAYear());
            var etagHeaders = GetEtagHeader(etag);
            foreach (var (key, value) in etagHeaders)
            {
                headers.Add(key, value);
            }
            return headers;
        }

        private static IDictionary<string, string> CacheAtCdnFor(int timeInSeconds) =>
            new Dictionary<string, string>
            {
                // Surrogate-Control header was added to support cache-control at the CDN level.
                // Here we want the CDN to cache the content for the specified amount of time.
                // Client will check in with the CDN to see if the content has changed using the ETag header and will get a response accordingly.
                // When the user record is updated, we will purge the CDN cache using their cache API.
                { "Surrogate-Control", $"max-age={timeInSeconds}" },
                { "Cache-Control", "no-cache" },

                // { "Cache-Control", $"max-age={timeInSeconds}" }, -> We could also add this in case we choose to introduce a small cache at the client.
            };

        private static int GetSecondsInAYear() => 365 * 24 * 60 * 60;
    }
}