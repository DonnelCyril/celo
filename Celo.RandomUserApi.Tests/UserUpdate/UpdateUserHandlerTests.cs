using System.Threading.Tasks;
using Celo.RandomUserApi.UnitTests.UserUpdate.TestFixture;
using Celo.RandomUserApi.UserManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Celo.RandomUserApi.UnitTests.UserUpdate.Scenarios;

namespace Celo.RandomUserApi.UnitTests.UserUpdate
{

    public enum Scenarios
    {
        UserNotFoundInUserStore,
        UserFoundWithDifferentETag,
        UpdatingUserIdFieldIsProhibited,
        ConcurrentUpdateConflicts,
        UserRecordDeletedBeforeUpdate,
        UserRecordSuccessfullyUpdated
    }

    [TestClass]
    public class UpdateUserHandlerTests
    {
        [DataRow(UserNotFoundInUserStore, DisplayName = nameof(UserNotFoundInUserStore))]
        [DataRow(UserFoundWithDifferentETag, DisplayName = nameof(UserFoundWithDifferentETag))]
        [DataRow(UpdatingUserIdFieldIsProhibited, DisplayName = nameof(UpdatingUserIdFieldIsProhibited))]
        [DataRow(ConcurrentUpdateConflicts, DisplayName = nameof(ConcurrentUpdateConflicts))]
        [DataRow(UserRecordDeletedBeforeUpdate, DisplayName = nameof(UserRecordDeletedBeforeUpdate))]
        [DataRow(UserRecordSuccessfullyUpdated, DisplayName = nameof(UserRecordSuccessfullyUpdated))]
        [DataTestMethod]
        public async Task Update_user_handler_scenarios(Scenarios scenario)
        {
            var (request, expectedResponse, dependencies) = Fixture.SetupForScenario(scenario);
            var (mediator, userStoreFactory, cdnCachePurge) = dependencies;
            var sut = new UpdateUserHandler(mediator, userStoreFactory, cdnCachePurge);
            var response = await sut.Handle(request, default);
            Assert.IsTrue(response.IsEquivalentTo(expectedResponse));
        }
    }



}
