using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Shipping.Queries;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IShippingRepository
    {
        Task<Shipping?> CreateAsync(Shipping shipping);
        Task<Shipping?> GetByOrderIdAsync(int orderId);
        Task<PagedResult<Shipping>> GetAllAsync(GetShippingsQuery query);
    }
}
