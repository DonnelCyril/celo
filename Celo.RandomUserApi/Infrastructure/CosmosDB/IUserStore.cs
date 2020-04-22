using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB.Model;
using Celo.RandomUserApi.UserManagement.Model.User;

namespace Celo.RandomUserApi.Infrastructure.CosmosDB
{
    public interface IUserStore
    {
        Task<UserRecord> CreateItemAsync(User user, CancellationToken cancellationToken = default);

        Task<UserRecord> ReadItemAsync(string userId, string etag, CancellationToken cancellationToken = default);

        Task<UserRecord> ReplaceItemAsync(User user, string etag, CancellationToken cancellationToken = default);

        Task<UserRecord> DeleteItemAsync(string userId, CancellationToken cancellationToken = default);

        Task<UserRecordIndex> QueryItemsAsync(Expression<Func<User, bool>> filter, string offset, CancellationToken cancellationToken = default);
    }
}