using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Customer.Models;
using OnlineStore.Application.Handlers.Customer.Queries;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<CustomerDetails?> GetByIdAsync(int id);
        Task<CustomerDetails?> GetByUserIdAsync(int id);
        Task<CustomerDetails> RegisterAsync(User user, Customer customer);
        Task<Customer?> GetByEmailAsync(string email);
        Task<PagedResult<CustomerDetails>> GetPagedAsync(GetCustomersQuery query);
        Task<bool> UpdateAsync(Customer customer);
    }
}
