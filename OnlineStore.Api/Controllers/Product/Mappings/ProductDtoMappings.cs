using OnlineStore.Api.Services;
using OnlineStore.Application.Dtos;

namespace OnlineStore.Api.Controllers.Product.Mappings
{
    public static class ProductDtoMappings
    {
        public static ProductDto WithFullImageUrls(this ProductDto dto, FileUrlGenerator generator)
        {
            dto.MainImageUrl = generator.GetUrl(dto.MainImageUrl);

            foreach (var image in dto.Images)
            {
                image.Url = generator.GetUrl(image.Url);
            }

            return dto;
        }
    }
}
