using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.Order.Requests;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Application.Handlers.Role;
using OnlineStore.Application.Handlers.Role.Queries;
using OnlineStore.Application.Handlers.Shipping.Commands;
using OnlineStore.Application.Handlers.Shipping;
using OnlineStore.Application.Security;
using OnlineStore.Api.Controllers.Role.Requests;
using OnlineStore.Application.Handlers.Role.Commands;

namespace OnlineStore.Api.Controllers.Role
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly GetRoleHandler _getRoleHandler;
        private readonly GetRolesHandler _getRolesHandler;
        private readonly CreateRoleHandler _createRoleHandler;

        public RolesController(GetRoleHandler getRoleHandler, GetRolesHandler getRolesHandler, CreateRoleHandler createRoleHandler)
        {
            _getRoleHandler = getRoleHandler;
            _getRolesHandler = getRolesHandler;
            _createRoleHandler = createRoleHandler;
        }

        [HttpGet("{id:int}", Name = "GetRoleById")]
        [Authorize(Policy = Permissions.Roles.View)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            return Ok(await _getRoleHandler.ExecuteAsync(new GetRoleQuery(id)));
        }

        [Authorize(Policy = Permissions.Roles.View)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            return Ok(await _getRolesHandler.ExecuteAsync(new GetRolesQuery()));
        }

        [HttpPost()]
        [Authorize(Policy = Permissions.Roles.Create)]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleRequest request)
        {
            var role = await _createRoleHandler.ExecuteAsync(new CreateRoleCommand(request.RoleName));
            return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
        }
    }
}
