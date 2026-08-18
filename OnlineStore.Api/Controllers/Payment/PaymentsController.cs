using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Payment;
using OnlineStore.Application.Handlers.Payment.Queries;
using OnlineStore.Application.Security;

namespace OnlineStore.Api.Controllers.Payment
{
    [ApiController]
    [Route("api/payments")]
    public sealed class PaymentsController : ControllerBase
    {
        private readonly GetPaymentsHandler _getPaymentsHandler;
        private readonly GetPaymentHandler _getPaymentByIdHandler;

        public PaymentsController(GetPaymentsHandler getPaymentsHandler, GetPaymentHandler getPaymentByIdHandler)
        {
            _getPaymentsHandler = getPaymentsHandler;
            _getPaymentByIdHandler = getPaymentByIdHandler;
        }

        [Authorize(Policy = Permissions.Payments.View)]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<PaymentDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDto<PaymentDto>>> GetAll([FromQuery] GetPaymentsQuery query)
        {
            return Ok(await _getPaymentsHandler.ExecuteAsync(query));
        }

        [Authorize(Policy = Permissions.Payments.View)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentDto>> GetById(int id)
        {
            return Ok(await _getPaymentByIdHandler.ExecuteAsync(new GetPaymentQuery(id)));
        }
    }
}