using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.Cdn;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.GetUser;
using Celo.RandomUserApi.UserManagement.Model.UpdateUser;
using MediatR;
using User = Celo.RandomUserApi.UserManagement.Model.User.User;

namespace Celo.RandomUserApi.UserManagement
{
    public delegate (List<string>, User) GetUpdatedUser(User user);
    public class UpdateUser : IRequest<IUpdateUserResult>
    {
        public UpdateUser(Guid userId, string etag, GetUpdatedUser getUpdatedUser)
        {
            getUpdatedUser.ThrowIfNull(nameof(getUpdatedUser));
            etag.ThrowIfNullOrEmpty(nameof(etag));
            UserId = userId;
            Etag = etag;
            GetUpdatedUser = getUpdatedUser;
        }

        public Guid UserId { get; }
        public string Etag { get; }
        public GetUpdatedUser GetUpdatedUser { get; }
    }

    public class UpdateUserHandler : IRequestHandler<UpdateUser, IUpdateUserResult>
    {
        private readonly IMediator _mediator;
        private readonly IUserStoreFactory _userStoreFactory;
        private readonly ICdnCachePurge _cdnCachePurge;

        public UpdateUserHandler(IMediator mediator, IUserStoreFactory userStoreFactory, ICdnCachePurge cdnCachePurge)
        {
            _mediator = mediator;
            _userStoreFactory = userStoreFactory;
            _cdnCachePurge = cdnCachePurge;
        }

        public async Task<IUpdateUserResult> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            var existingUser = await _mediator.Send(new GetUserById(request.UserId), cancellationToken);
            return existingUser switch
            {
                UserNotFound _ => UserNotFound.Instance,
                UserFound userFound when IsNotMatchingETags(userFound.Etag, request.Etag) => new NewerVersionOfUserRecordFound(userFound.Etag),
                UserFound userFound => await UpdateUser(request, userFound, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(existingUser))
            };

        }

        async Task<IUpdateUserResult> UpdateUser(UpdateUser request, UserFound userFound, CancellationToken cancellationToken)
        {
            var (errors, updatedUser) = request.GetUpdatedUser(userFound.User);
            if (UserIdFieldIsModified(userFound.User, updatedUser))
                errors.Add($"UserId modified from {userFound.User.UserId} to {updatedUser.UserId}. This field cannot be updated.");
            if (errors.Any()) return new FailedToApplyUserModifications(errors);
            var userStore = await _userStoreFactory.GetUserStore();
            var response = await userStore.ReplaceItemAsync(updatedUser, userFound.Etag, cancellationToken);
            return response.StatusCode switch
            {
                HttpStatusCode.PreconditionFailed => new NewerVersionOfUserRecordFound(response.ETag),
                HttpStatusCode.NotFound => UserNotFound.Instance,
                _ => await PurgeCdnCache(request.UserId, response.ETag)
            };
        }

        private async Task<UserSuccessfullyUpdated> PurgeCdnCache(Guid userId, string etag)
        {
            await _cdnCachePurge.PurgeUserRecord(userId);
            return new UserSuccessfullyUpdated(etag);
        }

        static bool UserIdFieldIsModified(User existingUser, User updatedUser) => existingUser.UserId != updatedUser.UserId;

        static bool IsNotMatchingETags(string existingTag, string newEtag) =>
            !existingTag.Trim('"').Equals(newEtag.Trim('"'), StringComparison.InvariantCultureIgnoreCase);
    }
}