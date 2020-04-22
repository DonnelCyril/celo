using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.Infrastructure.CosmosDB.Model;
using Celo.RandomUserApi.UserManagement.Model.User;

namespace Celo.RandomUserApi.IntegrationTests.Stubs
{
    public class StubUserStore : IUserStore
    {
        private readonly UserRecord _userRecord;

        public StubUserStore(UserRecord userRecord) => _userRecord = userRecord;

        public Task<UserRecord> CreateItemAsync(User user, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<UserRecord> ReadItemAsync(string userId, string etag, CancellationToken cancellationToken = default) => 
            Task.FromResult(new UserRecord(HttpStatusCode.OK,_userRecord.ETag,_userRecord.User));

        public Task<UserRecord> ReplaceItemAsync(User user, string etag, CancellationToken cancellationToken = default) => 
            Task.FromResult(new UserRecord(HttpStatusCode.NoContent, Guid.NewGuid().ToString(),user));

        public Task<UserRecord> DeleteItemAsync(string userId, CancellationToken cancellationToken = default) => 
            throw new NotImplementedException();

        public Task<UserRecordIndex> QueryItemsAsync(Expression<Func<User, bool>> filter, string offset, CancellationToken cancellationToken = default) => 
            throw new NotImplementedException();
    }
}