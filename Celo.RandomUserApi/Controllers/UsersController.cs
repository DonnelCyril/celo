using System.Threading.Tasks;
using Celo.RandomUserApi.Controllers.Model;
using Celo.RandomUserApi.Infrastructure.AutoMapper;
using Celo.RandomUserApi.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Celo.RandomUserApi.Infrastructure.ActionFilter.Model.ResponseWrapper;

namespace Celo.RandomUserApi.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDomainObjectMapper _mapper;

        public UsersController(IMediator mediator, IDomainObjectMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("page")]
        public async Task<IActionResult> Get([FromQuery]string offset)
        {
            var userIndex = await _mediator.Send(new GetUserList(offset));
            var (_, userIndexDto) = _mapper.MapTo<UserIndexDto>(userIndex);
            return new ObjectResult(ForUserIndexPage(userIndexDto));
        }

        [HttpGet]

        [Route("search")]
        public async Task<IActionResult> Search([FromQuery]string firstName, [FromQuery]string lastName, [FromQuery]string offset)
        {

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                return BadRequest(new ValidationProblemDetails { Detail = "Either firstName or lastName query parameters must be specified" });
            var searchUsersQuery = string.IsNullOrWhiteSpace(firstName)
                ? SearchUsers.ByLastName(lastName, offset)
                : SearchUsers.ByFirstName(firstName, offset);
            var userIndex = await _mediator.Send(searchUsersQuery);
            var (_, userIndexDto) = _mapper.MapTo<UserIndexDto>(userIndex);
            return Ok(userIndexDto);
        }

    }
}
