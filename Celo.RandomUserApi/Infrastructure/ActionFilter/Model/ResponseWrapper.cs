using System.Collections.Generic;
using System.Net;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using static Celo.RandomUserApi.Infrastructure.Cdn.CacheHeaders;

namespace Celo.RandomUserApi.Infrastructure.ActionFilter.Model
{
    public class ResponseWrapper
    {
        private ResponseWrapper(HttpStatusCode statusCode, IDictionary<string, string> headers, object responseBody = default)
        {
            headers.ThrowIfNull(nameof(headers));
            StatusCode = statusCode;
            ResponseBody = responseBody;
            Headers = headers;

        }
        public HttpStatusCode StatusCode { get; }
        public object ResponseBody { get; }
        public IDictionary<string, string> Headers { get; }

        public void Deconstruct(out HttpStatusCode statusCode, out IDictionary<string, string> headers, out object body)
        {
            statusCode = StatusCode;
            headers = Headers;
            body = ResponseBody;
        }

        public static ResponseWrapper PreconditionFailed = new ResponseWrapper(
            HttpStatusCode.PreconditionFailed,
           new Dictionary<string, string>(),
           new ProblemDetails
           {
               Title = "Concurrency conflict",
               Detail = "Newer version of the user record found. Re-fetch the user record and try the update again."
           });

        public static ResponseWrapper ForNewUser(UserDto userDto, string etag) =>
            new ResponseWrapper(HttpStatusCode.Created, GetEtagHeader(etag), userDto);

        public static ResponseWrapper ForExistingUser(UserDto userDto, string etag) =>
            new ResponseWrapper(HttpStatusCode.OK, GetUserCacheHeaders(userDto, etag), userDto);

        public static ResponseWrapper ForUpdatedUser(string etag) => new ResponseWrapper(HttpStatusCode.NoContent, GetEtagHeader(etag));

        public static ResponseWrapper ForUserIndexPage(UserIndexDto userIndexDto) =>
            new ResponseWrapper(HttpStatusCode.OK, GetUserIndexPageCacheHeaders(userIndexDto), userIndexDto);

    }
}