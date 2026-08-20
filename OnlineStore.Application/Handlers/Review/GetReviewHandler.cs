using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Review.Mappings;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class GetReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewHandler(IReviewRepository reviewRepository) => _reviewRepository = reviewRepository;

        public async Task<ReviewDto?> ExecuteAsync(GetReviewQuery query)
        {
            Domain.Entities.Review? review = await _reviewRepository.GetByIdAsync(query.ReviewId) ?? throw new NotFoundException("Review not found.");
            return review.ToDto();
        }
    }
}
