using OnlineStore.Application.Handlers.Customer.Models;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<CustomerDetails?> GetByIdAsync(int id);
        Task<CustomerDetails?> GetByUserIdAsync(int id);
    }
}
