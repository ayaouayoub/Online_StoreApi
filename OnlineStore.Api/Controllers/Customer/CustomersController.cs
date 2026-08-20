using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Customer;
using OnlineStore.Application.Handlers.Customer.Queries;
using OnlineStore.Infrastructure.Authorization;
using OnlineStore.Api.Controllers.Customer.Requests;
using OnlineStore.Application.Handlers.Customer.Commands;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Handlers.Review;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Api.Controllers.Review.Requests;

namespace OnlineStore.Api.Controllers.Customer
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly GetCustomerHandler _getCustomerHandler;
        private readonly GetCustomersHandler _getCustomersHandler;
        private readonly UpdateCustomerHandler _updateCustomerHandler;
        private readonly GetReviewsByCustomerIdHandler _getReviewsByCustomerIdHandler;
        private readonly ICurrentUser _currentUser;
        private readonly UpdateReviewHandler _updateReviewHandler;
        private readonly DeleteMyReviewHandler _deleteMyReviewHandler;

        public CustomersController(GetCustomerHandler getCustomerHandler, GetCustomersHandler getCustomersHandler, UpdateCustomerHandler updateCustomerHandler, GetReviewsByCustomerIdHandler getReviewsByCustomerIdHandler, ICurrentUser currentUser, UpdateReviewHandler updateReviewHandler, DeleteMyReviewHandler deleteMyReviewHandler = null)
        {
            _getCustomerHandler = getCustomerHandler;
            _getCustomersHandler = getCustomersHandler;
            _updateCustomerHandler = updateCustomerHandler;
            _getReviewsByCustomerIdHandler = getReviewsByCustomerIdHandler;
            _currentUser = currentUser;
            _updateReviewHandler = updateReviewHandler;
            _deleteMyReviewHandler = deleteMyReviewHandler;
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

        [HttpGet("{customerId:int}/reviews")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetCustomerReviews(int customerId)
        {
            return Ok(await _getReviewsByCustomerIdHandler.ExecuteAsync(new GetReviewsByCustomerIdQuery(customerId)));
        }

        [HttpGet("me/reviews")]
        [Authorize(Policy = Policies.CustomerOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> MyReviews()
        {
            return Ok(await _getReviewsByCustomerIdHandler.ExecuteAsync(new GetReviewsByCustomerIdQuery(_currentUser.UserId)));
        }

        [HttpPut("me/reviews/{reviewId:int}")]
        [Authorize(Policy = Policies.CustomerOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReview(int reviewId,[FromBody] UpdateReviewRequest request)
        {
            await _updateReviewHandler.ExecuteAsync(new UpdateReviewCommand
            (
                ReviewId: reviewId,
                Rating: request.Rating,
                ReviewText: request.ReviewText
            ));
            return NoContent();
        }

        [HttpDelete("me/reviews/{reviewId:int}")]
        [Authorize(Policy = Policies.CustomerOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMyReview(int reviewId, [FromBody] DeleteMyReviewRequest request)
        {
            await _deleteMyReviewHandler.ExecuteAsync(new DeleteMyReviewCommand(ReviewId: reviewId, DeleteReason: request.DeleteReason));
            return NoContent();
        }
    }
}
