using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.Order.Requests;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Order.Commands;
using OnlineStore.Application.Handlers.Order;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Application.Security;

namespace OnlineStore.Api.Controllers.Order
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderHandler _createOrderHandler;
        private readonly GetOrderHandler _getOrderHandler;
        private readonly PayOrderHandler _payOrderHandler;
        public OrdersController(CreateOrderHandler createOrderHandler, GetOrderHandler getOrderHandler, PayOrderHandler payOrderHandler)
        {
            _createOrderHandler = createOrderHandler;
            _getOrderHandler = getOrderHandler;
            _payOrderHandler = payOrderHandler;
        }

        [HttpPost]
        [Authorize(Policy = "Customer")]
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

        [HttpPost("{orderId:int}/pay")]
        [Authorize(Policy = "Customer")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Pay(int orderId, [FromBody] PayOrderRequest request) 
        { 
            return Ok(await _payOrderHandler.ExecuteAsync(new PayOrderCommand
            (
                OrderId: orderId,
                PaymentMethodId: request.PaymentMethodId,
                Provider: request.Provider
            ))); 
        }

        [HttpGet("{id:int}", Name = "GetOrderById")]
        [Authorize(Policy = Permissions.Orders.View)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            return Ok(await _getOrderHandler.ExecuteAsync(new GetOrderQuery(id)));
        }
    }
}
