using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Celo.RandomUserApi.Controllers;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.Infrastructure.ActionFilter.Model;
using Celo.RandomUserApi.Infrastructure.AutoMapper;
using Celo.RandomUserApi.Infrastructure.AutoMapper.MapperProfiles;
using Celo.RandomUserApi.UserManagement;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Celo.RandomUserApi.UnitTests.UserUpdate
{
    [TestClass]
    public class PatchActionTest
    {
        private IDomainObjectMapper _domainObjectMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _domainObjectMapper = CreateDomainObjectMapper();
        }

        [DataRow(nameof(FailedToApplyUserModifications), HttpStatusCode.BadRequest, DisplayName = "Returns 400 BadRequest if the patch modification failed.")]
        [DataRow(nameof(NewerVersionOfUserRecordFound), HttpStatusCode.PreconditionFailed, DisplayName = "Returns 412 Precondition failed if there are concurrency conflicts when updating a record.")]
        [DataRow(nameof(UserSuccessfullyUpdated), HttpStatusCode.NoContent, DisplayName = "Returns 204 when a user record is successfully updated.")]
        [DataRow(nameof(UserNotFound), HttpStatusCode.NotFound, DisplayName = "Returns 404 when the specified user record cannot be found.")]
        [DataTestMethod]
        public async Task Correct_responses_are_for_different_scenarios(string scenario, HttpStatusCode expectedStatusCode)
        {
            var mediator = CreateMediatorForUpdateUserRequest(scenario);
            var sut = new UserController(mediator, _domainObjectMapper);
            var response = await sut.Patch(Guid.NewGuid(), Guid.NewGuid().ToString(), new JsonPatchDocument<UserDto>());
            var responseCode = GetResponseCode(response);
            Assert.AreEqual((int)expectedStatusCode, responseCode);
        }

        private static IMediator CreateMediatorForUpdateUserRequest(string scenario)
        {
            IUpdateUserResult response = scenario switch
            {
                nameof(FailedToApplyUserModifications) => new FailedToApplyUserModifications(new []{"Failed modification"}),
                nameof(NewerVersionOfUserRecordFound) => new NewerVersionOfUserRecordFound(Guid.NewGuid().ToString()),
                nameof(UserSuccessfullyUpdated) => new UserSuccessfullyUpdated(Guid.NewGuid().ToString()),
                nameof(UserNotFound) => UserNotFound.Instance
            };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUser>(), default)).ReturnsAsync(response);
            return mediatorMock.Object;
        }

        private static int GetResponseCode(IActionResult actionResult)
        {
            return actionResult switch
            {
                BadRequestObjectResult badRequest => badRequest.StatusCode.Value,
                ObjectResult objectResult => (int)(objectResult.Value as ResponseWrapper).StatusCode,
                NotFoundResult notFoundResult => notFoundResult.StatusCode
            };
        }

        private static IDomainObjectMapper CreateDomainObjectMapper()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile<CreateUserProfile>();
                opts.AddProfile<UserIndexProfile>();
                opts.AddProfile<UserProfile>();
            });
            var mapper = config.CreateMapper();
            var domainObjectMapper = new DomainObjectMapper(mapper);
            return domainObjectMapper;
        }
    }
}
