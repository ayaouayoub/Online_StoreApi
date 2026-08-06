using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Category.Mappings;
using OnlineStore.Application.Handlers.Product.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Product;

public sealed class GetProductsHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResultDto<ProductDto>> ExecuteAsync(GetProductsQuery query)
    {
        PagedResult<Domain.Entities.Product> result = await _productRepository.GetProductsAsync(query);

        return new PagedResultDto<ProductDto>
        {
            TotalCount = result.TotalCount,
            Page = query.Page,
            PageSize = query.PageSize,

            Items =
            [
                ..result.Items.Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    QuantityInStock = product.QuantityInStock,
                    MainImageUrl = product.MainImageUrl,

                    Images =
                    [
                        ..product.Images.Select(image => new ProductImageDto
                        {
                            Url = image.Url,
                            ImageOrder = image.ImageOrder
                        })
                    ],

                    Category = product.Category!.ToDto()
                })
            ]
        };
    }
}