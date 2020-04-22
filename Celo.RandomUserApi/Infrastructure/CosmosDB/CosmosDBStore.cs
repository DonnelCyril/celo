using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using User = Celo.RandomUserApi.UserManagement.Model.User.User;

namespace Celo.RandomUserApi.Infrastructure.CosmosDB
{
    public class CosmosDBUserStore : IUserStore
    {
        private readonly Container _container;
        private readonly PartitionKey _partitionKey = new PartitionKey(Constants.UserRecords.PartitionKey);

        public CosmosDBUserStore(string endpointUri, string primaryKey)
        {
            var cosmosClient = new CosmosClient(endpointUri, primaryKey);
            _container = cosmosClient.GetContainer("demo-db", "users");
        }

        public Task<UserRecord> CreateItemAsync(User user, CancellationToken cancellationToken = default) =>
            Try(() => _container.CreateItemAsync(user, cancellationToken: cancellationToken));

        public Task<UserRecord> ReadItemAsync(string userId, string etag, CancellationToken cancellationToken = default) =>
           Try(() => _container.ReadItemAsync<User>(userId, _partitionKey, GetRequestOptions(etag), cancellationToken));

        public Task<UserRecord> ReplaceItemAsync(User user, string etag, CancellationToken cancellationToken = default) =>
            Try(() => _container.ReplaceItemAsync(user, user.UserId.ToString(), _partitionKey, GetRequestOptions(etag), cancellationToken));

        public Task<UserRecord> DeleteItemAsync(string userId, CancellationToken cancellationToken = default) =>
            Try(() => _container.DeleteItemAsync<User>(userId, _partitionKey, cancellationToken: cancellationToken));

        public async Task<UserRecordIndex> QueryItemsAsync(Expression<Func<User, bool>> filter, string offset = default, CancellationToken cancellationToken = default)
        {
            var iterator = _container.GetItemLinqQueryable<User>(continuationToken: offset)
                .Where(filter)
                .Select(u => u.UserId)
                .ToFeedIterator();

            var response = await iterator.ReadNextAsync(cancellationToken);
            var userRecordsIndex = new UserRecordIndex(response.Resource.Select(g => g).ToList(), response.ContinuationToken);
            return userRecordsIndex;
        }

        private static ItemRequestOptions GetRequestOptions(string etag) => new ItemRequestOptions { IfNoneMatchEtag = etag };

        // We need to wrap Cosmos DB call in a try catch as it will throw if the response code is not a 200.
        // We want to handle cases like 304, 404, 412 to be treated as normal scenarios and not throw exceptions.
        private static async Task<UserRecord> Try(Func<Task<ItemResponse<User>>> getUser)
        {
            try
            {
                var itemResponse = await getUser();
                return new UserRecord(itemResponse.StatusCode, itemResponse.ETag, itemResponse.Resource);
            }

            catch (CosmosException ex) when (
                ex.StatusCode == HttpStatusCode.NotFound ||
                ex.StatusCode == HttpStatusCode.NotModified ||
                ex.StatusCode == HttpStatusCode.PreconditionFailed
            )
            {
                return new UserRecord(ex.StatusCode, ex.Headers.ETag);
            }
        }
    }
}
