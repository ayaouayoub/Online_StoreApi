using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Payment.Queries;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<PagedResult<Payment>> GetPagedAsync(GetPaymentsQuery query);
        Task<Payment?> GetByIdAsync(int id);
    }
}
