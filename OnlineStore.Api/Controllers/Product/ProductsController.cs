using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Product;
using OnlineStore.Api.Controllers.Product.Requests;
using OnlineStore.Application.Handlers.Product.Commands;
using OnlineStore.Application.Interfaces.Services;
using OnlineStore.Api.Controllers.Product.Mappings;
using OnlineStore.Api.Services;
using OnlineStore.Application.Handlers.Product.Queries;

namespace OnlineStore.Api.Controllers.Product
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CreateProductHandler _createProductHandler;
        private readonly IImageStorageService _imageStorageService;
        private readonly FileUrlGenerator _fileUrlGenerator;
        private readonly GetProductHandler _getProductHandler;

        public ProductsController(CreateProductHandler createProductHandler, IImageStorageService imageStorageService, FileUrlGenerator fileUrlGenerator, GetProductHandler getProductHandler)
        {
            _createProductHandler = createProductHandler;
            _imageStorageService = imageStorageService;
            _fileUrlGenerator = fileUrlGenerator;
            _getProductHandler = getProductHandler;
        }

        [Authorize(Policy = Permissions.Products.Create)]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] CreateProductRequest request)
        {
            string? mainImageUrl = null;

            if (request.MainImage is not null)
            {
                mainImageUrl = await _imageStorageService.SaveAsync(request.MainImage);
            }

            List<CreateProductImageCommand> images = [];

            short order = 1;

            if (request.Images is not null)
            {
                foreach (IFormFile image in request.Images)
                {
                    images.Add(new CreateProductImageCommand(await _imageStorageService.SaveAsync(image), order++));
                }
            }

            CreateProductCommand command = new
            (
                request.Name,
                request.Description,
                request.Price,
                request.QuantityInStock,
                mainImageUrl,
                request.CategoryId,
                images
            );

            ProductDto dto = await _createProductHandler.ExecuteAsync(command);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto.WithFullImageUrls(_fileUrlGenerator));
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            ProductDto dto = await _getProductHandler.ExecuteAsync(new GetProductQuery(id));
            return Ok(dto.WithFullImageUrls(_fileUrlGenerator));
        }
    }
}
