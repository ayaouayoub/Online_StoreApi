using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Customer;
using OnlineStore.Application.Handlers.Customer.Queries;
using OnlineStore.Infrastructure.Authorization;

namespace OnlineStore.Api.Controllers.Customer
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly GetCustomerHandler _getCustomerHandler;

        public CustomersController(GetCustomerHandler getCustomerHandler)
        {
            _getCustomerHandler = getCustomerHandler;
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
    }
}
