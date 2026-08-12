using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Handlers.User;
using OnlineStore.Api.Controllers.Authentication.Requests;
using OnlineStore.Application.Handlers.Customer.Commands;
using OnlineStore.Application.Handlers.Customer.Models;
using OnlineStore.Application.Handlers.Customer;

namespace OnlineStore.Api.Controllers.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginHandler _login;
        private readonly RegisterCustomerHandler _registerCustomerHandler;

        public AuthController(LoginHandler login, RegisterCustomerHandler registerCustomerHandler)
        {
            _login = login;
            _registerCustomerHandler = registerCustomerHandler;
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

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CustomerDetails), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDetails>> Register([FromBody] RegisterCustomerRequest request)
        {
            var details = await _registerCustomerHandler.ExecuteAsync
            (
                new RegisterCustomerCommand
                (
                    Name: request.Name,
                    Username: request.Username,
                    Password: request.Password,
                    Email: request.Email,
                    Address: request.Address,
                    Phone: request.Phone
                )
             );

            return CreatedAtRoute("GetCustomerById", new { id = details.Customer.Id }, details);
        }
    }
}
