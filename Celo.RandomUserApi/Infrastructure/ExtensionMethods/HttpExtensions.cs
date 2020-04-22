using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Celo.RandomUserApi.Infrastructure.ExtensionMethods
{
    public static class HttpExtensions
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
 
        public static Task WriteJson<T>(this HttpResponse response, T obj, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json";
            return response.WriteAsync(JsonConvert.SerializeObject(obj, Settings));
        }
    }
}