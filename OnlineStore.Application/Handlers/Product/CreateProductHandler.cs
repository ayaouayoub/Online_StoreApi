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
    public sealed class CreateProductHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageStorageService _imageStorageService;

        public CreateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageStorageService imageStorageService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageStorageService = imageStorageService;
        }

        public async Task<ProductDto> ExecuteAsync(CreateProductCommand command)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId) ?? throw new NotFoundException("Category not found.");

            if (!category.IsActive) throw new DomainException("Category is inactive.");


            List<string> savedImageUrls = [];

            try
            {
                string? mainImageUrl = null;

                if (command.MainImage is not null)
                {
                    mainImageUrl = await _imageStorageService.SaveAsync(command.MainImage);
                    savedImageUrls.Add(mainImageUrl);
                }

                var product = Domain.Entities.Product.Create
                (
                    command.Name,
                    command.Price,
                    command.QuantityInStock,
                    category,
                    command.Description,
                    mainImageUrl
                );

                foreach (CreateProductImageCommand image in command.Images)
                {
                    string imageUrl =await _imageStorageService.SaveAsync(image.Image);
                    savedImageUrls.Add(imageUrl);
                    product.AddImage(ProductImage.Create(imageUrl, image.ImageOrder));
                }

                var createdProduct = await _productRepository.CreateProductAsync(product);


                return createdProduct.ToDto();
            }
            catch
            {
                foreach (string imageUrl in savedImageUrls)
                {
                    try
                    {
                        await _imageStorageService.DeleteAsync(imageUrl);
                    }
                    catch { }
                }

                throw;
            }
        }
    }
}