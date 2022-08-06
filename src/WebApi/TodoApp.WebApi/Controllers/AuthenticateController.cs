using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoApp.Application.Dto.User;
using TodoApp.Application.Features.Commands.User;
using TodoApp.Application.Wappers;

namespace TodoApp.WebApi.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthenticateController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse<LoginResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
            => Ok(await mediator.Send(new Login.Command { Request = request }));

    }
}
