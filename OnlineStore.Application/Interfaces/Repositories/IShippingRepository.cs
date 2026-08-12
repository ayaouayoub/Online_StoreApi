using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IShippingRepository
    {
        Task<Shipping?> CreateAsync(Shipping shipping);
        Task<Shipping?> GetByOrderIdAsync(int orderId);
    }
}
