using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.PaymentMethod.Commands;
using OnlineStore.Application.Handlers.PaymentMethod.Queries;
using OnlineStore.Application.Handlers.PaymentMethod;
using OnlineStore.Api.Controllers.PaymentMethod.Requests;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Application.Security;

namespace OnlineStore.Api.Controllers.PaymentMethod
{
    [Route("api/payment-methods")]
    [ApiController]
    public sealed class PaymentMethodsController : ControllerBase
    {
        private readonly CreatePaymentMethodHandler _createPaymentMethodHandler;
        private readonly GetPaymentMethodsHandler _getPaymentMethodsHandler;
        private readonly GetPaymentMethodHandler _getPaymentMethodHandler;
        private readonly UpdatePaymentMethodHandler _updatePaymentMethodHandler;
        private readonly DeactivatePaymentMethodHandler _deactivatePaymentMethodHandler;
        private readonly ActivatePaymentMethodHandler _activatePaymentMethodHandler;

        public PaymentMethodsController(CreatePaymentMethodHandler createPaymentMethodHandler, GetPaymentMethodsHandler getPaymentMethodsHandler, GetPaymentMethodHandler getPaymentMethodHandler, UpdatePaymentMethodHandler updatePaymentMethodHandler, DeactivatePaymentMethodHandler deactivatePaymentMethodHandler, ActivatePaymentMethodHandler activatePaymentMethodHandler)
        {
            _createPaymentMethodHandler = createPaymentMethodHandler;
            _getPaymentMethodsHandler = getPaymentMethodsHandler;
            _getPaymentMethodHandler = getPaymentMethodHandler;
            _updatePaymentMethodHandler = updatePaymentMethodHandler;
            _deactivatePaymentMethodHandler = deactivatePaymentMethodHandler;
            _activatePaymentMethodHandler = activatePaymentMethodHandler;
        }

        [Authorize(Policy = Permissions.PaymentMethods.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentMethodDto>> Create([FromBody] CreatePaymentMethodRequest request)
        {
            var paymentMethod = await _createPaymentMethodHandler.ExecuteAsync(new CreatePaymentMethodCommand(request.Name, request.Description));
            return CreatedAtAction(nameof(GetById), new { id = paymentMethod.Id }, paymentMethod);
        }

        [Authorize(Policy = Permissions.PaymentMethods.View)]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PaymentMethodDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyCollection<PaymentMethodDto>>> GetAll()
        {
            return Ok(await _getPaymentMethodsHandler.ExecuteAsync(new GetPaymentMethodsQuery()));
        }

        [Authorize(Policy = Permissions.PaymentMethods.View)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentMethodDto>> GetById(int id)
        {
            return Ok(await _getPaymentMethodHandler.ExecuteAsync(new GetPaymentMethodQuery(id)));
        }

        [Authorize(Policy = Permissions.PaymentMethods.Update)]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentMethodDto>> Update(int id, [FromBody] UpdatePaymentMethodRequest request)
        {
            return Ok(await _updatePaymentMethodHandler.ExecuteAsync(new UpdatePaymentMethodCommand(id, request.Description)));
        }

        [Authorize(Policy = Permissions.PaymentMethods.Delete)]
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _deactivatePaymentMethodHandler.ExecuteAsync(new DeactivatePaymentMethodCommand(id));
            return NoContent();
        }

        [Authorize(Policy = Permissions.PaymentMethods.Update)]
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivatePaymentMethod(int id)
        {
            await _activatePaymentMethodHandler.ExecuteAsync(new ActivatePaymentMethodCommand(id));
            return NoContent();
        }
    }
}
