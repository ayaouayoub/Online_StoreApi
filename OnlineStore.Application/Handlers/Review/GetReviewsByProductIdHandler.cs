using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Review.Mappings;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class GetReviewsByProductIdHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewsByProductIdHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IReadOnlyList<ReviewDto>> ExecuteAsync(GetReviewsByProductIdQuery query)
        {
            IReadOnlyList<Domain.Entities.Review> reviews = await _reviewRepository.GetByProductIdAsync(query.ProductId);
            return [..reviews.Select(x => x.ToDto())];
        }
    }
}
