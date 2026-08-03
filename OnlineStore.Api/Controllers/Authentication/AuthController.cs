using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Handlers.User;
using OnlineStore.Api.Controllers.Authentication.Requests;

namespace OnlineStore.Api.Controllers.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginHandler _login;

        public AuthController(LoginHandler login)
        {
            _login = login;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequest Request)
        {
            return Ok(await _login.ExecuteAsync(new LoginCommand
            (
                Request.Username,
                Request.Password
            )));
        }
    }
}
