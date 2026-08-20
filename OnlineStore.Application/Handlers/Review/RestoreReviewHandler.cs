using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class RestoreReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public RestoreReviewHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task ExecuteAsync(RestoreReviewCommand command)
        {
            var review = await _reviewRepository.GetByIdAsyncIncludingDeleted(command.ReviewId) ?? throw new NotFoundException("Review not found.");

            review.Restore();

            await _reviewRepository.RestoreAsync(review);
        }
    }
}
