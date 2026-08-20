using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.Review.Mappings
{
    public static class ReviewMappings
    {
        public static ReviewAdminDto ToAdminDto(this Domain.Entities.Review review)
        {
            return new ReviewAdminDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = review.Product?.Name ?? string.Empty,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer?.User?.Name ?? string.Empty,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                IsDeleted = review.IsDeleted,
                DeletedAt = review.DeletedAt,
                DeletedById = review.DeletedById,
                DeletedByName = review.DeletedByUser?.Name,
                DeleteReason = review.DeleteReason
            };
        }


        public static ReviewDto ToDto(this Domain.Entities.Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = review.Product?.Name ?? string.Empty,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer?.User?.Name ?? string.Empty,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }
    }
}
