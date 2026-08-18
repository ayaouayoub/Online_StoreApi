using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Category.Commands;
using OnlineStore.Application.Handlers.Category.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Category
{
    public sealed class UpdateCategoryHandler
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> ExecuteAsync(UpdateCategoryCommand command)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId) ?? throw new NotFoundException("Category not found.");

            Domain.Entities.Category? parent = null;

            if (command.ParentCategoryId.HasValue)
            {
                parent = await _categoryRepository.GetByIdAsync(command.ParentCategoryId.Value) ?? throw new NotFoundException("Parent category not found.");
                if (!parent.IsActive) throw new DomainException("Parent category is inactive.");
            }

            category.ChangeName(command.Name);
            category.ChangeDescription(command.Description);
            category.ChangeParent(parent);
            category.ChangeDisplayOrder(command.DisplayOrder);

            if (!await _categoryRepository.UpdateAsync(category)) throw new DomainException("Failed to update category.");

            return category.ToDto();
        }
    }
}