using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Product.Commands;
using OnlineStore.Application.Handlers.Product.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces.Services.Images;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Product
{
    public sealed class UpdateProductHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageStorageService _imageStorageService;

        public UpdateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageStorageService imageStorageService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageStorageService = imageStorageService;
        }

        public async Task<ProductDto> ExecuteAsync(UpdateProductCommand command)
        {
            var product = await _productRepository.GetByIdAsync(command.ProductId) ?? throw new NotFoundException("Product not found.");

            var category = await _categoryRepository.GetByIdAsync(command.CategoryId) ?? throw new NotFoundException("Category not found.");

            if (!category.IsActive) throw new DomainException("Cannot assign product to an inactive category.");

            string? mainImageUrl = product.MainImageUrl;

            if (command.MainImage is not null)
            {
                mainImageUrl = await _imageStorageService.SaveAsync(command.MainImage);
            }

            string? oldMainImageUrl = product.MainImageUrl;
            if (oldMainImageUrl is not null) await _imageStorageService.DeleteAsync(oldMainImageUrl);

            foreach (var oldImage in product.Images)
            {
                await _imageStorageService.DeleteAsync(oldImage.Url);
            }

            product.RemveAllImages();

            foreach (UpdateProductImageCommand imageCommand in command.Images)
            {
                string imageUrl = await _imageStorageService.SaveAsync(imageCommand.Image);
                product.AddImage(ProductImage.Create(imageUrl, imageCommand.ImageOrder));
            }

            product.ChangeName(command.Name);
            product.ChangeDescription(command.Description);
            product.ChangePrice(command.Price);
            product.ChangeCategory(category);
            product.ChangeMainImage(mainImageUrl);

            var updatedProduct = await _productRepository.UpdateAsync(product);

            return updatedProduct.ToDto();
        }
    }
}
