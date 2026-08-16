using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.User.Requests;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.User;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Handlers.User.Queries;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Security;
using OnlineStore.Infrastructure.Authorization;

namespace OnlineStore.Api.Controllers.User
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GetUserHandler _getUserHandler;
        private readonly GetCurrentUserHandler _getCurrentUserHandler;
        private readonly CreateUserHandler _createUserHandler;
        private readonly GetUsersHandler _getUsersHandler;
        private readonly DeactivateUserHandler _deactivateUserHandler;
        private readonly ActivateUserHandler _activateUserHandler;
        private readonly ChangeMyPasswordHandler _changeMyPasswordHandler;
        private readonly UpdateUserHandler _updateUserHandler;
        private readonly ICurrentUser _currentUser;

        public UsersController(GetUserHandler getUserHandler, GetCurrentUserHandler getCurrentUserHandler, CreateUserHandler createUserHandler, GetUsersHandler getUsersHandler, DeactivateUserHandler deactivateUserHandler, ActivateUserHandler activateUserHandler, ChangeMyPasswordHandler changeMyPasswordHandler, UpdateUserHandler updateUserHandler, ICurrentUser currentUser)
        {
            _getUserHandler = getUserHandler;
            _getCurrentUserHandler = getCurrentUserHandler;
            _createUserHandler = createUserHandler;
            _getUsersHandler = getUsersHandler;
            _deactivateUserHandler = deactivateUserHandler;
            _activateUserHandler = activateUserHandler;
            _changeMyPasswordHandler = changeMyPasswordHandler;
            _updateUserHandler = updateUserHandler;
            _currentUser = currentUser;
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

        [HttpPost]
        [Authorize(Policy = Permissions.Users.Create)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
        {
            var user = await _createUserHandler.ExecuteAsync(new CreateUserCommand 
            {
                Name = request.Name,
                Username = request.Username,
                Password = request.Password,
                PermissionIds = request.PermissionIds
            });
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] GetUsersQuery query)
        {
            return Ok(await _getUsersHandler.ExecuteAsync(query));
        }


        [Authorize(Policy = Permissions.Users.Delete)]
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            await _deactivateUserHandler.ExecuteAsync(new DeactivateUserCommand(id));
            return NoContent();
        }

        [Authorize(Policy = Permissions.Users.Update)]
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivateUser(int id)
        {
            await _activateUserHandler.ExecuteAsync(new ActivateUserCommand(id));
            return NoContent();
        }

        [Authorize]
        [HttpPatch("me/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangenMyPassword(ChangeMyPasswordRequest request)
        {
            await _changeMyPasswordHandler.ExecuteAsync(new ChangeMyPasswordCommand(request.CurrentPassword, request.NewPassword));
            return NoContent();
        }

        [Authorize(Policy = Permissions.Users.Update)]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            return Ok(await _updateUserHandler.ExecuteAsync(new UpdateUserCommand
            {
                UserId = id,
                Name = request.Name,
                Username = request.Username
            }));
        }

        [Authorize]
        [HttpPatch("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> UpdateMyInfo([FromBody] UpdateUserRequest request)
        {
            return Ok(await _updateUserHandler.ExecuteAsync(new UpdateUserCommand
            {
                UserId = _currentUser.UserId,
                Name = request.Name,
                Username = request.Username
            }));
        }
    }
}
