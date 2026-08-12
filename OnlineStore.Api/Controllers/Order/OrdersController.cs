using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.Order.Requests;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Order.Commands;
using OnlineStore.Application.Handlers.Order;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Shipping.Commands;
using OnlineStore.Application.Handlers.Shipping;
using OnlineStore.Application.Handlers.Shipping.Queries;
using OnlineStore.Infrastructure.Authorization;

namespace OnlineStore.Api.Controllers.Order
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderHandler _createOrderHandler;
        private readonly GetOrderHandler _getOrderHandler;
        private readonly PayOrderHandler _payOrderHandler;
        private readonly CreateShippingHandler _createShippingHandler;
        private readonly GetOrderShippingHandler _getOrderShippingHandler;
        private readonly ShipOrderHandler _shipOrderHandler;
        private readonly DeliverOrderHandler _deliverOrderHandler;

        public OrdersController(CreateOrderHandler createOrderHandler, GetOrderHandler getOrderHandler, PayOrderHandler payOrderHandler, CreateShippingHandler createShippingHandler, GetOrderShippingHandler getOrderShippingHandler, ShipOrderHandler shipOrderHandler, DeliverOrderHandler deliverOrderHandler)
        {
            _createOrderHandler = createOrderHandler;
            _getOrderHandler = getOrderHandler;
            _payOrderHandler = payOrderHandler;
            _createShippingHandler = createShippingHandler;
            _getOrderShippingHandler = getOrderShippingHandler;
            _shipOrderHandler = shipOrderHandler;
            _deliverOrderHandler = deliverOrderHandler;
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

        [HttpPost("{orderId:int}/shipping")]
        [Authorize(Policy = Permissions.Shipping.Create)]
        [ProducesResponseType(typeof(ShippingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShippingDto>> CreateShipping(int orderId, [FromBody] CreateShippingRequest request)
        {
            var shipping = await _createShippingHandler.ExecuteAsync
            (
               new CreateShippingCommand
               (
                    OrderId: orderId,
                    CarrierName: request.CarrierName,
                    TrackingNumber: request.TrackingNumber,
                    EstimatedDeliveryDate: request.EstimatedDeliveryDate
               )
            );

            return CreatedAtAction(nameof(GetOrderShipping), new { orderId }, shipping);
        }

        [HttpPost("{orderId:int}/shipping/ship")]
        [Authorize(Policy = Permissions.Shipping.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ShipOrder(int orderId)
        {
            await _shipOrderHandler.ExecuteAsync(new ShipOrderCommand(orderId));
            return NoContent();
        }

        [HttpPost("{orderId:int}/shipping/deliver")]
        [Authorize(Policy = Permissions.Shipping.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            await _deliverOrderHandler.ExecuteAsync(new DeliverOrderCommand(orderId));
            return NoContent();
        }

        [HttpGet("{orderId:int}/shipping")]
        [Authorize(Policy = Policies.ShippingView)]
        [ProducesResponseType(typeof(ShippingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShippingDto>> GetOrderShipping(int orderId)
        {
            return Ok(await _getOrderShippingHandler.ExecuteAsync(new GetOrderShippingQuery(orderId)));
        } 
    }
}
