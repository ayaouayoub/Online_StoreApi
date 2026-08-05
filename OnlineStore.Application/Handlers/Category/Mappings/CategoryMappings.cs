using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.Category.Mappings
{
    public static class CategoryMappings
    {
        public static CategoryDto ToDto(this Domain.Entities.Category category)
        {
            if (category is null)
                return null!;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder
            };
        }
    }
}
