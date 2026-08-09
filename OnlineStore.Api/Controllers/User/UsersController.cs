using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.User;
using OnlineStore.Application.Handlers.User.Queries;
using OnlineStore.Application.Security;

namespace OnlineStore.Api.Controllers.User
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GetUserHandler _getUserHandler;
        private readonly GetCurrentUserHandler _getCurrentUserHandler;

        public UsersController(GetUserHandler getUserHandler, GetCurrentUserHandler getCurrentUserHandler)
        {
            _getUserHandler = getUserHandler;
            _getCurrentUserHandler = getCurrentUserHandler;
        }

        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            return Ok(await _getUserHandler.ExecuteAsync(new GetUserByIdQuery(id)));
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDto> Me()
        {
            return Ok(_getCurrentUserHandler.Execute());
        }
    }
}
