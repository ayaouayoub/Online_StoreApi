using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Review.Mappings;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class GetReviewsByCustomerIdHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewsByCustomerIdHandler(
            IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IReadOnlyList<ReviewDto>> ExecuteAsync(GetReviewsByCustomerIdQuery query)
        {
            IReadOnlyList<Domain.Entities.Review> reviews = await _reviewRepository.GetByCustomerIdAsync(query.CustomerId);
            return [..reviews.Select(x => x.ToDto())];
        }
    }
}
