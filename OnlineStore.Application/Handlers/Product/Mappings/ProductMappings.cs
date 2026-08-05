using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Category.Mappings;
using OnlineStore.Application.Handlers.Product.Commands;

namespace OnlineStore.Application.Handlers.Product.Mappings
{
    public static class ProductMappings
    {
        public static ProductDto ToDto(this CreateProductCommand command, Domain.Entities.Category category, int productId)
        {
            return new ProductDto
            {
                Name = command.Name,
                Id = productId,
                QuantityInStock = command.QuantityInStock,
                Description = command.Description,
                Price = command.Price,
                MainImageUrl = command.MainImageUrl,

                Images = [.. command.Images.Select(i => new ProductImageDto
                {
                    Url = i.Url,
                    ImageOrder = i.ImageOrder,
                })],

                Category = category.ToDto(),
                CategoryId = category.Id
            };
        }
    }
}
