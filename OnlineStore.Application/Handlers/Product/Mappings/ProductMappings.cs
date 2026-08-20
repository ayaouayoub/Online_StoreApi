using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Category.Mappings;
using OnlineStore.Application.Handlers.Product.Commands;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Handlers.Product.Mappings
{
    public static class ProductMappings
    {
        public static ProductDto ToDto(this Domain.Entities.Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                MainImageUrl = product.MainImageUrl,

                Images =
                [
                    .. product.Images.Select(i => new ProductImageDto
                {
                    Url = i.Url,
                    ImageOrder = i.ImageOrder
                })
                ],
                
                Category = product.Category?.ToDto(),
                CategoryId = product.CategoryId
            };
        }
    }
}
