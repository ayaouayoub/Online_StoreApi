using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Category.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Category
{
    public sealed class ActivateCategoryHandler
    {
        private readonly ICategoryRepository _categoryRepository;

        public ActivateCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task ExecuteAsync(ActivateCategoryCommand command)
        {
            var category =  await _categoryRepository.GetByIdAsync(command.CategoryId) ?? throw new NotFoundException("Category not found.");

            category.Activate();

            if (category.ParentId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(category.ParentId.Value) ?? throw new DomainException("Parent category does not exist.");
                if (!parent.IsActive) throw new DomainException("Cannot activate category while its parent is inactive.");
            }

            if (!await _categoryRepository.ActivateAsync(command.CategoryId)) throw new DomainException("Failed to activate category.");
        }
    }
}