using System.Net;
using Celo.RandomUserApi.UserManagement.Model.User;

namespace Celo.RandomUserApi.Infrastructure.CosmosDB.Model
{
    public class UserRecord
    {
        public UserRecord(HttpStatusCode statusCode, string eTag, User user = default)
        {
            StatusCode = statusCode;
            ETag = eTag;
            User = user;
        }

        public HttpStatusCode StatusCode { get; }
        public string ETag { get; }
        public User User { get; }

    }
}