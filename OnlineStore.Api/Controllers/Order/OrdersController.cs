using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.Order.Requests;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Order.Commands;
using OnlineStore.Application.Handlers.Order;

namespace OnlineStore.Api.Controllers.Order
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderHandler _createOrderHandler;

        public OrdersController(CreateOrderHandler createOrderHandler)
        {
            _createOrderHandler = createOrderHandler;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request)
        {
            var command = new CreateOrderCommand
            {
                Items = 
                [.. request.Items.Select(x => new CreateOrderItemCommand 
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })]
            };

            var order = await _createOrderHandler.ExecuteAsync(command);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
