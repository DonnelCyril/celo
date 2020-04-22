using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.Cdn;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.DeleteUser;
using MediatR;

namespace Celo.RandomUserApi.UserManagement
{
    public class DeleteUserById : IRequest<IUserDeleteResult>
    {
        public DeleteUserById(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }

    public class DeleteUserByIdHandler : IRequestHandler<DeleteUserById, IUserDeleteResult>
    {
        private readonly IUserStoreFactory _userStoreFactory;
        private readonly ICdnCachePurge _cdnCachePurge;

        public DeleteUserByIdHandler(IUserStoreFactory userStoreFactory, ICdnCachePurge cdnCachePurge)
        {
            _userStoreFactory = userStoreFactory;
            _cdnCachePurge = cdnCachePurge;
        }

        public async Task<IUserDeleteResult> Handle(DeleteUserById request, CancellationToken cancellationToken)
        {

            var userStore = await _userStoreFactory.GetUserStore();
            var response = await userStore.DeleteItemAsync(request.UserId.ToString(), cancellationToken);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => UserNotFound.Instance,
                _ => await PurgeCdnCache(request.UserId)
            };

            async Task<UserDeleted> PurgeCdnCache(Guid userId)
            {
                await _cdnCachePurge.PurgePagesWithUser(userId);
                return UserDeleted.Instance;
            }
        }

    }
}