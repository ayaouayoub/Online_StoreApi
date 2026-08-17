using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<int> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task CreateFailedPaymentAndCancelOrderAsync(Payment payment, int id);
        Task CreatePaymentAndMarkAsPaidAsync(Payment payment, Order order);
        Task ShipAsync(int orderId);
        Task DeliverAsync(int orderId);
        Task<PagedResult<Order>> GetPagedAsync(GetOrdersQuery query);
    }
}
