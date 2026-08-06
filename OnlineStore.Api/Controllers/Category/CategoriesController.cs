using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Category;
using OnlineStore.Application.Handlers.Category.Queries;
using OnlineStore.Application.Handlers.Product.Queries;

namespace OnlineStore.Api.Controllers.Category
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly GetCategoryHandler _getCategoryHandler;
        private readonly GetCategoriesHandler _getCategoriesHandler;

        public CategoriesController(GetCategoryHandler getCategoryHandler, GetCategoriesHandler getCategoriesHandler)
        {
            _getCategoryHandler = getCategoryHandler;
            _getCategoriesHandler = getCategoriesHandler;
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            CategoryDto dto = await _getCategoryHandler.ExecuteAsync(new GetCategoryQuery(id));
            return Ok(dto);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategorySummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<CategorySummaryDto>>> GetCategories()
        {
            var categories = await _getCategoriesHandler.ExecuteAsync(new GetCategoriesQuery());

            return Ok(categories);
        }
    }
}
