using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Category.Commands;
using OnlineStore.Application.Handlers.Category.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Category
{
    public sealed class CreateCategoryHandler
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> ExecuteAsync(CreateCategoryCommand command)
        {
            Domain.Entities.Category? parent = null;

            if (command.ParentCategoryId.HasValue)
            {
                parent = await _categoryRepository.GetByIdAsync(command.ParentCategoryId.Value) ?? throw new NotFoundException("Parent category not found.");
                if (!parent.IsActive) throw new DomainException("Parent category is inactive.");
            }

            var category = Domain.Entities.Category.Create
            (
                name: command.Name,
                description: command.Description,
                parent: parent,
                displayOrder: command.DisplayOrder
            );

            var createdCategory = await _categoryRepository.CreateAsync(category);

            return createdCategory.ToDto();
        }
    }
}