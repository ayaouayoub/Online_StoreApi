using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task CreateFailedPaymentAndCancelOrderAsync(Payment payment, int id);
        Task CreatePaymentAndMarkAsPaidAsync(Payment payment, Order order);
        Task ShipAsync(int orderId);
        Task DeliverAsync(int orderId);
        Task<PagedResult<Order>> GetPagedAsync(GetOrdersQuery query);
    }
}
