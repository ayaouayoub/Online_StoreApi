using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Exceptions;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class DeleteReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUser _currentUser;

        public DeleteReviewHandler(IReviewRepository reviewRepository, ICurrentUser currentUser)
        {
            _reviewRepository = reviewRepository;
            _currentUser = currentUser;
        }

        public async Task ExecuteAsync(DeleteReviewCommand command)
        {
            var review = await _reviewRepository.GetByIdAsync(command.ReviewId) ?? throw new NotFoundException("Review not found.");
            review.Delete(_currentUser.User, command.DeleteReason);
            await _reviewRepository.DeleteAsync(review);
        }
    }
}
