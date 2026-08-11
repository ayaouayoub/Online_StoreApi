using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<int> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task CreateFailedPaymentAndCancelOrderAsync(Payment payment, int id);
        Task CreatePaymentAndMarkAsPaidAsync(Payment payment, Order order);
    }
}
