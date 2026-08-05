using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Product.Mappings;
using OnlineStore.Application.Handlers.Product.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Product
{
    public sealed class GetProductHandler
    {
        private readonly IProductRepository _repository;

        public GetProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> ExecuteAsync(GetProductQuery query)
        {
            Domain.Entities.Product product = await _repository.GetByIdAsync(query.ProductId) ?? throw new NotFoundException("Product not found.");

            return product.ToDto();
        }
    }
}
