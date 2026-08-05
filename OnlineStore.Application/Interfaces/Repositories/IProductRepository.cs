using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<int> GetByIdAsync(Product product);
        Task<int> CreateProductAsync(Product product);
    }
}
