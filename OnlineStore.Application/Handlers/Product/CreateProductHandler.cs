using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Product.Commands;
using OnlineStore.Application.Handlers.Product.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Product
{
    public sealed class CreateProductHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDto> ExecuteAsync(CreateProductCommand command)
        {
            Domain.Entities.Category category = await _categoryRepository.GetByIdAsync(command.CategoryId) ?? throw new NotFoundException("Category not found.");

            if (!category.IsActive) throw new DomainException("Category is inactive.");

            Domain.Entities.Product product = Domain.Entities.Product.Create
            (
                command.Name,
                command.Price,
                command.QuantityInStock,
                category,
                command.Description,
                command.MainImageUrl
            );

            foreach (var image in command.Images)
            {
                product.AddImage(ProductImage.Create(image.Url, image.ImageOrder));
            }

            int productId = await _productRepository.CreateProductAsync(product);

            return command.ToDto(category, productId);
        }
    }
}
