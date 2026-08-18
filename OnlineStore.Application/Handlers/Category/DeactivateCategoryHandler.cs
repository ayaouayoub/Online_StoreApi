using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Category.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Category
{
    public sealed class DeactivateCategoryHandler
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeactivateCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task ExecuteAsync(DeactivateCategoryCommand command)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId)?? throw new NotFoundException("Category not found.");

            category.Deactivate();

            if (await _categoryRepository.HasActiveChildrenAsync(category.Id)) throw new DomainException("Cannot deactivate a category that has active child categories.");

            if (!await _categoryRepository.DeactivateAsync(category.Id)) throw new DomainException( "Failed to deactivate category.");
        }
    }
}