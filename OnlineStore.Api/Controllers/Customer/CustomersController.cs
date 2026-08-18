using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Customer;
using OnlineStore.Application.Handlers.Customer.Queries;
using OnlineStore.Infrastructure.Authorization;
using OnlineStore.Api.Controllers.Customer.Requests;
using OnlineStore.Application.Handlers.Customer.Commands;

namespace OnlineStore.Api.Controllers.Customer
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly GetCustomerHandler _getCustomerHandler;
        private readonly GetCustomersHandler _getCustomersHandler;
        private readonly UpdateCustomerHandler _updateCustomerHandler;

        public CustomersController(GetCustomerHandler getCustomerHandler, GetCustomersHandler getCustomersHandler, UpdateCustomerHandler updateCustomerHandler)
        {
            _getCustomerHandler = getCustomerHandler;
            _getCustomersHandler = getCustomersHandler;
            _updateCustomerHandler = updateCustomerHandler;
        }

        [Authorize(Policy = Policies.CustomerView)]
        [HttpGet("{id:int}", Name = "GetCustomerById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
        {
            return Ok(await _getCustomerHandler.ExecuteAsync(new GetCustomerQuery(id)));
        }

        [Authorize(Policy = Permissions.Customers.View)]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDto<CustomerDto>>> GetAll([FromQuery] GetCustomersQuery query)
        {
            return Ok(await _getCustomersHandler.ExecuteAsync(query));
        }

        [Authorize(Policy = Permissions.Customers.Update)]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> Update(int id, [FromBody] UpdateCustomerRequest request)
        {
            return Ok(await _updateCustomerHandler.ExecuteAsync(new UpdateCustomerCommand
            {
                CustomerId = id,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address
            }));
        }
    }
}
