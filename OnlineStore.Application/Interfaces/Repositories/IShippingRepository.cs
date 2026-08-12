using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IShippingRepository
    {
        Task<Shipping?> CreateAsync(Shipping shipping);
        Task ShipAsync(int orderId);
        Task DeliverAsync(int orderId);
        Task<Shipping?> GetByOrderIdAsync(int orderId);
    }
}
