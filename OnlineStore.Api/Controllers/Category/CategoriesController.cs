using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Api.Controllers.Category.Requests;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Category;
using OnlineStore.Application.Handlers.Category.Commands;
using OnlineStore.Application.Handlers.Category.Queries;
using OnlineStore.Application.Handlers.Product.Queries;
using OnlineStore.Application.Security;

namespace OnlineStore.Api.Controllers.Category
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly GetCategoryHandler _getCategoryHandler;
        private readonly GetCategoriesHandler _getCategoriesHandler;
        private readonly CreateCategoryHandler _createCategoryHandler;
        private readonly UpdateCategoryHandler _updateCategoryHandler;
        private readonly ActivateCategoryHandler _activateCategoryHandler;
        private readonly DeactivateCategoryHandler _deactivateCategoryHandler;

        public CategoriesController(GetCategoryHandler getCategoryHandler, GetCategoriesHandler getCategoriesHandler, CreateCategoryHandler createCategoryHandler, UpdateCategoryHandler updateCategoryHandler, ActivateCategoryHandler activateCategoryHandler, DeactivateCategoryHandler deactivateCategoryHandler)
        {
            _getCategoryHandler = getCategoryHandler;
            _getCategoriesHandler = getCategoriesHandler;
            _createCategoryHandler = createCategoryHandler;
            _updateCategoryHandler = updateCategoryHandler;
            _activateCategoryHandler = activateCategoryHandler;
            _deactivateCategoryHandler = deactivateCategoryHandler;
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

        [Authorize(Policy = Permissions.Categories.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
        {
            var category = await _createCategoryHandler.ExecuteAsync(new CreateCategoryCommand
            {
                Name = request.Name,
                Description = request.Description,
                ParentCategoryId = request.ParentCategoryId,
                DisplayOrder = request.DisplayOrder
            });

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [Authorize(Policy = Permissions.Categories.Update)]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            return Ok(await _updateCategoryHandler.ExecuteAsync(new UpdateCategoryCommand
            {
                CategoryId = id,
                Name = request.Name,
                Description = request.Description,
                ParentCategoryId = request.ParentCategoryId,
                DisplayOrder = request.DisplayOrder
            }));
        }

        [Authorize(Policy = Permissions.Categories.Update)]
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Activate(int id)
        {
            await _activateCategoryHandler.ExecuteAsync(new ActivateCategoryCommand(id));
            return NoContent();
        }

        [Authorize(Policy = Permissions.Categories.Update)]
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _deactivateCategoryHandler.ExecuteAsync(new DeactivateCategoryCommand(id));
            return NoContent();
        }
    }
}