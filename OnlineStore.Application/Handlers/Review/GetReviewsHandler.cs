using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Review.Mappings;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class GetReviewsHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewsHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<PagedResultDto<ReviewAdminDto>> ExecuteAsync(GetReviewsQuery query)
        {
            PagedResult<Domain.Entities.Review> result =await _reviewRepository.GetAllAsync(query);

            return new PagedResultDto<ReviewAdminDto>
            {
                Items = [.. result.Items.Select(ReviewMappings.ToAdminDto)],
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
