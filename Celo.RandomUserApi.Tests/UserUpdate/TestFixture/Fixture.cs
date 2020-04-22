using System;
using System.Collections.Generic;
using System.Net;
using Celo.RandomUserApi.Infrastructure.Cdn;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.Infrastructure.CosmosDB.Model;
using Celo.RandomUserApi.UserManagement;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.GetUser;
using Celo.RandomUserApi.UserManagement.Model.UpdateUser;
using Celo.RandomUserApi.UserManagement.Model.User;
using MediatR;
using Moq;
using static Celo.RandomUserApi.UnitTests.UserUpdate.Scenarios;

namespace Celo.RandomUserApi.UnitTests.UserUpdate.TestFixture
{
    public class Fixture
    {
        public static (UpdateUser, IUpdateUserResult, UpdateUserHandlerDependencies) SetupForScenario(Scenarios scenario)
        {
            var request = GetRequestForScenario(scenario);
            var expectedResponse = GetExpectedResultForScenario(scenario, request);
            var dependencies = ConfigureDependenciesForScenario(request, expectedResponse);
            return (request, expectedResponse, dependencies);
        }

        private static readonly Guid _updatedUserId = Guid.NewGuid();
        private static UpdateUser GetRequestForScenario(Scenarios scenario) =>
            scenario switch
            {
                UpdatingUserIdFieldIsProhibited => new UpdateUser(
                    User.UserId,
                    $"etag-{scenario}",
                    user =>
                    {
                        var updatedUser = new User(_updatedUserId, user.Name, user.ContactInfo, user.DateOfBirth, user.ProfileImages);
                        return (new List<string>(), updatedUser);
                    }
                ),
                _ => new UpdateUser(User.UserId, $"etag-{scenario}", user => (new List<string>(), user))
            };

        private static IUpdateUserResult GetExpectedResultForScenario(Scenarios scenario, UpdateUser request) =>
            scenario switch
            {
                UserNotFoundInUserStore => UserNotFound.Instance,
                UserFoundWithDifferentETag => new NewerVersionOfUserRecordFound($"etag-{scenario}"),
                UpdatingUserIdFieldIsProhibited => new FailedToApplyUserModifications(
                    new[] { $"UserId modified from {request.UserId} to {_updatedUserId}. This field cannot be updated." }),
                ConcurrentUpdateConflicts => new NewerVersionOfUserRecordFound($"etag-{scenario}"),
                UserRecordDeletedBeforeUpdate => UserNotFound.Instance,
                UserRecordSuccessfullyUpdated => new UserSuccessfullyUpdated($"etag-{scenario}"),
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

        private static UpdateUserHandlerDependencies ConfigureDependenciesForScenario(UpdateUser request, IUpdateUserResult response)
        {
            var mediatorMock = new Mock<IMediator>();
            var userStoreFactoryMock = new Mock<IUserStoreFactory>();
            var cdnCachePurge = new Mock<ICdnCachePurge>();
            switch (response)
            {
                case FailedToApplyUserModifications _:
                    ConfigureMediatorForGetUserCall(mediatorMock, request.Etag);
                    break;
                case NewerVersionOfUserRecordFound newerVersionOfUserRecordFound:
                    ConfigureMediatorForGetUserCall(mediatorMock, newerVersionOfUserRecordFound.Etag);
                    ConfigureUserStoreForReplaceItemCall(userStoreFactoryMock, newerVersionOfUserRecordFound.Etag, HttpStatusCode.PreconditionFailed);
                    break;
                case UserSuccessfullyUpdated userSuccessfullyUpdated:
                    ConfigureMediatorForGetUserCall(mediatorMock, request.Etag);
                    ConfigureUserStoreForReplaceItemCall(userStoreFactoryMock, userSuccessfullyUpdated.Etag, HttpStatusCode.NoContent);
                    break;
                case UserNotFound userNotFound:
                    mediatorMock.Setup(m => m.Send(It.IsAny<GetUserById>(), default)).ReturnsAsync(userNotFound);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(response));
            }
            return new UpdateUserHandlerDependencies(mediatorMock.Object, userStoreFactoryMock.Object, cdnCachePurge.Object);
        }

        private static void ConfigureMediatorForGetUserCall(Mock<IMediator> mediatorMock, string etag) => 
            mediatorMock.Setup(m => m.Send(It.IsAny<GetUserById>(), default)).ReturnsAsync(new UserFound(etag, User));

        private static void ConfigureUserStoreForReplaceItemCall(Mock<IUserStoreFactory> userStoreFactoryMock, string etag, HttpStatusCode statusCode)
        {
            var mockUserStore = new Mock<IUserStore>();
            mockUserStore.Setup(s => s.ReplaceItemAsync(It.IsAny<User>(), It.IsAny<string>(), default)).ReturnsAsync(new UserRecord(statusCode, etag));
            userStoreFactoryMock.Setup(s => s.GetUserStore()).ReturnsAsync(mockUserStore.Object);
        }

        private static readonly User User =
            new User(Guid.NewGuid(), new UserName("Master", "Aaron", "Donnel"), new ContactInfo("test@test.com", "09421434"), DateTime.Now, new List<Uri>());

    }
}
