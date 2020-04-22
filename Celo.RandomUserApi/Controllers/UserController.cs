using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.Infrastructure.AutoMapper;
using Celo.RandomUserApi.UserManagement;
using Celo.RandomUserApi.UserManagement.Model;
using Celo.RandomUserApi.UserManagement.Model.GetUser;
using Celo.RandomUserApi.UserManagement.Model.UpdateUser;
using Celo.RandomUserApi.UserManagement.Model.User;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static Celo.RandomUserApi.Infrastructure.ActionFilter.Model.ResponseWrapper;
using ValidationProblemDetails = Microsoft.AspNetCore.Mvc.ValidationProblemDetails;

namespace Celo.RandomUserApi.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IDomainObjectMapper _mapper;

        public UserController(IMediator mediator, IDomainObjectMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId, [FromHeader(Name = "If-None-Match")]string etag)
        {
            var userResult = await _mediator.Send(new GetUserById(userId, etag));
            var response = userResult switch
            {
                UserFound userFound => GetUserFoundResult(userFound),
                UserNotModified _ => new StatusCodeResult((int)HttpStatusCode.NotModified),
                UserNotFound _ => NotFound(),
                _ => throw new ArgumentOutOfRangeException(nameof(userResult))
            };
            return response;

            IActionResult GetUserFoundResult(UserFound userFound)
            {
                var (errorMessage, userDto) = _mapper.MapTo<UserDto>(userFound.User);
                if (!string.IsNullOrWhiteSpace(errorMessage)) throw new Exception(errorMessage);
                return new ObjectResult(ForExistingUser(userDto, userFound.Etag));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDto createUserDto)
        {
            var (errorMessage, createUser) = _mapper.MapTo<CreateUser>(createUserDto);
            if (!string.IsNullOrWhiteSpace(errorMessage)) return BadRequest(errorMessage);
            var userRecord = await _mediator.Send(createUser);
            var (_, userDto) = _mapper.MapTo<UserDto>(userRecord.User);
            return new ObjectResult(ForNewUser(userDto, userRecord.Etag));
        }

        [HttpPatch]
        [Route("{userId}")]
        public async Task<IActionResult> Patch(Guid userId, [FromHeader(Name = "If-Match")]string etag, [FromBody] JsonPatchDocument<UserDto> patchDoc)
        {
            if (string.IsNullOrWhiteSpace(etag))
                return new ObjectResult(new ValidationProblemDetails {Detail = "User record's ETag value is expected in the If-Match header"})
                {
                    StatusCode = (int) HttpStatusCode.PreconditionRequired
                };
            var updateUserRequest = new UpdateUser(userId, etag, GetUpdatedUser);
            var updateUserResult = await _mediator.Send(updateUserRequest);
            IActionResult response = updateUserResult switch
            {
                FailedToApplyUserModifications userRecord => BadRequest(userRecord.Errors),
                NewerVersionOfUserRecordFound _ => new ObjectResult(PreconditionFailed),
                UserSuccessfullyUpdated userRecord => new ObjectResult(ForUpdatedUser(userRecord.Etag)),
                UserNotFound _ => NotFound(),
                _ => throw new ArgumentOutOfRangeException(nameof(updateUserResult))
            };
            return response;

            (List<string>, User) GetUpdatedUser(User existingUser)
            {
                // map the internal User model to the API model for the user (UserDto).
                var (errorMessage, existingUserDto) = _mapper.MapTo<UserDto>(existingUser);
                if (!string.IsNullOrWhiteSpace(errorMessage)) throw new Exception(errorMessage);
                // this makes it easy to call patchDoc.ApplyTo to create an updated UserDto based on the patchDoc object.
                patchDoc.ApplyTo(existingUserDto, ModelState);
                // if there are errors, exit early.
                if (!ModelState.IsValid)
                    return (ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), null);
                // Otherwise map the Dto back to the domain model user, which we will now attempt to replace in Cosmos DB.
                var (_, updatedUser) = _mapper.MapTo<User>(existingUserDto);
                return (new List<string>(), updatedUser);
            }
        }

        [HttpDelete]
        [Route("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var response = await _mediator.Send(new DeleteUserById(userId));
            return response switch
            {
                UserNotFound _ => BadRequest($"UserId {userId} cannot be found"),
                _ => NoContent()
            };
        }

    }
}