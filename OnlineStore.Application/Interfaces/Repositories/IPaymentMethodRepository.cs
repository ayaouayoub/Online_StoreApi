using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod?> GetByIdAsync(int id);
        Task<IReadOnlyCollection<PaymentMethod>> GetAllAsync();
        Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod);
        Task<bool> UpdateAsync(PaymentMethod paymentMethod);
        Task<bool> DeactivateAsync(int id);
        Task<bool> ActivateAsync(int id);
    }
}
