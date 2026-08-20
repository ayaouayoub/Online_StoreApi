using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Product.Queries;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<PagedResult<Product>> GetProductsAsync(GetProductsQuery query);
        Task UpdateStockAsync(int productId, int quantityChange);
        Task<Product> UpdateAsync(Product product);
    }
}
