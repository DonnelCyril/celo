using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.UserManagement.Model.GetUserList;
using Celo.RandomUserApi.UserManagement.Model.User;
using MediatR;

namespace Celo.RandomUserApi.UserManagement
{
    public class SearchUsers : IRequest<UserIndex>
    {
        private SearchUsers(Expression<Func<User, bool>> filter, string offset)
        {
            Filter = filter;
            Offset = offset;
        }

        public static SearchUsers ByFirstName(string firstName, string offset)
            => new SearchUsers(user => user.Name.FirstName.StartsWith(firstName), offset);

        public static SearchUsers ByLastName(string lastName, string offset)
            => new SearchUsers(user => user.Name.FirstName.StartsWith(lastName), offset);

        public Expression<Func<User, bool>> Filter { get; }

        public string Offset { get; }
    }

    public class SearchUsersHandler : IRequestHandler<SearchUsers, UserIndex>
    {
        private readonly IUserStoreFactory _userStoreFactory;

        public SearchUsersHandler(IUserStoreFactory userStoreFactory) => _userStoreFactory = userStoreFactory;

        public async Task<UserIndex> Handle(SearchUsers request, CancellationToken cancellationToken)
        {
            var userStore = await _userStoreFactory.GetUserStore();
            var result = await userStore.QueryItemsAsync(request.Filter, request.Offset, cancellationToken);
            return new UserIndex(result.UserIds, nextIndex: result.NextOffset, previousIndex: request.Offset);
        }
    }
}