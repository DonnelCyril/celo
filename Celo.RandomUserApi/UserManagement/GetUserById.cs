using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.GetUser;
using MediatR;

namespace Celo.RandomUserApi.UserManagement
{
    public class GetUserById : IRequest<IGetUserResult>
    {
        public GetUserById(Guid userId, string etag = default)
        {
            UserId = userId;
            Etag = etag;
        }

        public Guid UserId { get; }
        public string Etag { get; }
    }

    public class GetUserByIdHandler : IRequestHandler<GetUserById, IGetUserResult>
    {
        private readonly IUserStoreFactory _userStoreFactory;

        public GetUserByIdHandler(IUserStoreFactory userStoreFactory)
        {
            _userStoreFactory = userStoreFactory;
        }
        public async Task<IGetUserResult> Handle(GetUserById request, CancellationToken cancellationToken)
        {
            var userStore = await _userStoreFactory.GetUserStore();
            var response = await userStore.ReadItemAsync(request.UserId.ToString(), request.Etag, cancellationToken);
            return response.StatusCode switch
            {
                HttpStatusCode.NotModified => UserNotModified.Instance,
                HttpStatusCode.NotFound => UserNotFound.Instance,
                _ => new UserFound(response.ETag, response.User)
            };
        }
    }
}
