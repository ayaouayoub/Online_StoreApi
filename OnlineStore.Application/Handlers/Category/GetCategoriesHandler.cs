using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Category.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Category
{
    public sealed class GetCategoriesHandler
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<IReadOnlyList<CategorySummaryDto>> ExecuteAsync(GetCategoriesQuery query)
        {
            return _categoryRepository.GetCategoriesAsync();
        }
    }
}
