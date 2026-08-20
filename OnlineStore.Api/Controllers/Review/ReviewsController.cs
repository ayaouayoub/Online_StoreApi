using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Handlers.Review;
using OnlineStore.Application.Security;
using OnlineStore.Api.Controllers.Review.Requests;
using OnlineStore.Application.Handlers.Review.Commands;

namespace OnlineStore.Api.Controllers.Review
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly GetReviewsHandler _getReviewsHandler;
        private readonly GetReviewHandler _getReviewHandler;
        private readonly DeleteReviewHandler _deleteReviewHandler;
        private readonly RestoreReviewHandler _restoreReviewHandler;

        public ReviewsController(GetReviewsHandler getReviewsHandler, GetReviewHandler getReviewHandler, DeleteReviewHandler deleteReviewHandler, RestoreReviewHandler restoreReviewHandler)
        {
            _getReviewsHandler = getReviewsHandler;
            _getReviewHandler = getReviewHandler;
            _deleteReviewHandler = deleteReviewHandler;
            _restoreReviewHandler = restoreReviewHandler;
        }

        [Authorize(Policy = Permissions.Reviews.View)]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ReviewAdminDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDto<ReviewAdminDto>>> GetReviews([FromQuery] GetReviewsQuery query)
        {
            return Ok(await _getReviewsHandler.ExecuteAsync(query));
        }

        [HttpGet("reviews/{reviewId:int}", Name = "GetReviewById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReviewDto>> GetById(int reviewId)
        {
            return Ok(await _getReviewHandler.ExecuteAsync(new GetReviewQuery(reviewId)));
        }

        [HttpDelete("reviews/{reviewId:int}")]
        [Authorize(Policy = Permissions.Reviews.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteReview(int reviewId, [FromBody] DeleteReviewRequest request)
        {
            await _deleteReviewHandler.ExecuteAsync(new DeleteReviewCommand(ReviewId: reviewId, DeleteReason: request.DeleteReason));
            return NoContent();
        }

        [HttpPost("{reviewId:int}/restore")]
        [Authorize(Policy = Permissions.Reviews.Restore)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RestoreReview(int reviewId)
        {
            await _restoreReviewHandler.ExecuteAsync(new RestoreReviewCommand(reviewId));
            return NoContent();
        }
    }
}
