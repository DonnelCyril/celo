using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.UserManagement.Model.GetUserList;
using MediatR;

namespace Celo.RandomUserApi.UserManagement
{
    public class GetUserList : IRequest<UserIndex>
    {
        public GetUserList(string offset)
        {
            Offset = offset;
        }
        public string Offset { get; }
    }

    public class GetUserListHandler : IRequestHandler<GetUserList, UserIndex>
    {
        private readonly IUserStoreFactory _userStoreFactory;

        public GetUserListHandler(IUserStoreFactory userStoreFactory)
        {
            _userStoreFactory = userStoreFactory;
        }
        public async Task<UserIndex> Handle(GetUserList request, CancellationToken cancellationToken)
        {
            var userStore = await _userStoreFactory.GetUserStore();
            var results = await userStore.QueryItemsAsync(_ => true, request.Offset, cancellationToken);
            return new UserIndex(results.UserIds, nextIndex: results.NextOffset, previousIndex: request.Offset);
        }
    }
}