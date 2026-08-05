using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Category.Mappings;
using OnlineStore.Application.Handlers.Category.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Category
{
    public sealed class GetCategoryHandler
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> ExecuteAsync(GetCategoryQuery query)
        {
            var category = await _categoryRepository.GetByIdAsync(query.CategoryId) ?? throw new NotFoundException("Category not found.");
            return category.ToDto();
        }
    }
}
