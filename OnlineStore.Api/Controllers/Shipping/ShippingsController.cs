using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Shipping.Queries;
using OnlineStore.Application.Handlers.Shipping;
using OnlineStore.Infrastructure.Authorization;
using OnlineStore.Application.Security;

namespace OnlineStore.Api.Controllers.Shipping
{
    [Route("api/shippings")]
    [ApiController]
    public class ShippingsController : ControllerBase
    {
        private readonly GetShippingsHandler _getShippingsHandler;
        private readonly GetShippingHander _getShippingHander;

        public ShippingsController(GetShippingsHandler getShippingsHandler, GetShippingHander getShippingHander)
        {
            _getShippingsHandler = getShippingsHandler;
            _getShippingHander = getShippingHander;
        }

        [Authorize(Policy = Permissions.Shipping.View)]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<ShippingDto>>> GetShippings([FromQuery] GetShippingsQuery query)
        {
            return Ok(await _getShippingsHandler.ExecuteAsync(query));
        }

        [Authorize(Policy = Permissions.Shipping.View)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShippingDto>> GetShipping(int id)
        {
            return Ok(await _getShippingHander.ExecuteAsync(new GetShippingQuery(id)));
        }
    }
}
