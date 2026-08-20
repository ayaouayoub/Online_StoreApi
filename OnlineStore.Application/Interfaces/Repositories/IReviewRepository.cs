using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<PagedResult<Review>> GetAllAsync(GetReviewsQuery query);
        Task<Review?> GetByIdAsync(int reviewId);
        Task<IReadOnlyList<Review>> GetByProductIdAsync(int productId);
        Task<IReadOnlyList<Review>> GetByCustomerIdAsync(int customerId);
        Task<Review> CreateAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task RestoreAsync(Review review);
        Task<Review?> GetByIdAsyncIncludingDeleted(int reviewId);
    }
}
