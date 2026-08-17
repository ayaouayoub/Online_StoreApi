using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.Role.Requests;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Permission;
using OnlineStore.Application.Handlers.Role.Commands;
using OnlineStore.Application.Handlers.Role.Queries;
using OnlineStore.Application.Handlers.Role;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Permission.Queries;
using OnlineStore.Api.Controllers.Permission.Requests;
using OnlineStore.Application.Handlers.Permission.Commands;

namespace OnlineStore.Api.Controllers.Permission
{
    [Route("api/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly GetPermissionHandler _getPermissionHandler;
        private readonly GetPermissionsHandler _getPermissionsHandler;
        private readonly CreatePermissionHandler _createPermissionHandler;

        public PermissionsController(GetPermissionHandler getPermissionHandler, GetPermissionsHandler getPermissionsHandler, CreatePermissionHandler createPermissionHandler)
        {
            _getPermissionHandler = getPermissionHandler;
            _getPermissionsHandler = getPermissionsHandler;
            _createPermissionHandler = createPermissionHandler;
        }

        [HttpGet("{id:int}", Name = "GetPermissionById")]
        [Authorize(Policy = Permissions.UserPermissions.View)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionDto>> GetById(int id)
        {
            return Ok(await _getPermissionHandler.ExecuteAsync(new GetPermissionQuery(id)));
        }

        [Authorize(Policy = Permissions.UserPermissions.View)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
            return Ok(await _getPermissionsHandler.ExecuteAsync(new GetPermissionsQuery()));
        }

        [HttpPost()]
        [Authorize(Policy = Permissions.UserPermissions.Create)]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionDto>> CreateRole([FromBody] CreatePermissionRequest request)
        {
            var permission = await _createPermissionHandler.ExecuteAsync(new CreatePermissionCommand
            {
                Code = request.Code,
                Name = request.Name
            });
            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }
    }
}
