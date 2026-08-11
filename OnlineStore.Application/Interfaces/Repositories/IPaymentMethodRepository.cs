using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod?> GetByIdAsync(int id);
    }
}
