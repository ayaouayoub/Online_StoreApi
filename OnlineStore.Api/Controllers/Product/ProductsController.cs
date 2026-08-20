using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Security;
using OnlineStore.Application.Handlers.Product;
using OnlineStore.Api.Controllers.Product.Requests;
using OnlineStore.Application.Handlers.Product.Commands;
using OnlineStore.Api.Controllers.Product.Mappings;
using OnlineStore.Api.Services;
using OnlineStore.Application.Handlers.Product.Queries;
using OnlineStore.Application.Interfaces.Services.Images;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Handlers.Review;
using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Infrastructure.Authorization;

namespace OnlineStore.Api.Controllers.Product
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CreateProductHandler _createProductHandler;
        private readonly FileUrlGenerator _fileUrlGenerator;
        private readonly GetProductHandler _getProductHandler;
        private readonly GetProductsHandler _getProductsHandler;
        private readonly UpdateStockHandler _updateStockHandler;
        private readonly GetReviewsByProductIdHandler _getReviewsByProductIdHandler;
        private readonly CreateReviewHandler _createReviewHandler;
        private readonly UpdateProductHandler _updateProductHandler;

        public ProductsController(CreateProductHandler createProductHandler, FileUrlGenerator fileUrlGenerator, GetProductHandler getProductHandler, GetProductsHandler getProductsHandler, UpdateStockHandler updateStockHandler, GetReviewsByProductIdHandler getReviewsByProductIdHandler, CreateReviewHandler createReviewHandler, UpdateProductHandler updateProductHandler)
        {
            _createProductHandler = createProductHandler;
            _fileUrlGenerator = fileUrlGenerator;
            _getProductHandler = getProductHandler;
            _getProductsHandler = getProductsHandler;
            _updateStockHandler = updateStockHandler;
            _getReviewsByProductIdHandler = getReviewsByProductIdHandler;
            _createReviewHandler = createReviewHandler;
            _updateProductHandler = updateProductHandler;
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
            List<CreateProductImageCommand> images = [];

            short order = 1;

            foreach (IFormFile image in request.Images)
            {
                images.Add(new CreateProductImageCommand(image, order++));
            }

            CreateProductCommand command = new
            (
                request.Name,
                request.Description,
                request.Price,
                request.QuantityInStock,
                request.MainImage,
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

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts([FromQuery] GetProductsQuery query)
        {
            var resultDto = await _getProductsHandler.ExecuteAsync(query);

            foreach (ProductDto item in resultDto.Items)
            {
                item.MainImageUrl = _fileUrlGenerator.GetUrl(item.MainImageUrl);

                item.Images = [.. item.Images.Select(image => image with
                {
                    Url = _fileUrlGenerator.GetUrl(image.Url)
                })];
            }

            return Ok(resultDto);
        }

        [HttpPatch("{id:int}/stock")]
        [Authorize(Policy = Permissions.Products.Update)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> UpdateStock(int id, [FromBody] UpdateStockRequest request)
        {
            return Ok(await _updateStockHandler.ExecuteAsync(new UpdateStockCommand
            (
                ProductId: id,
                QuantityChange: request.QuantityChange
            )));
        }

        [HttpGet("{productId:int}/reviews")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetProductReviews(int productId)
        {
            return Ok(await _getReviewsByProductIdHandler.ExecuteAsync(new GetReviewsByProductIdQuery(productId)));
        }

        [HttpPost("{productId:int}/review")]
        [Authorize(Policy = Policies.CustomerOnly)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ReviewDto>> CreateReview(int productId, [FromBody] CreateReviewRequest request)
        {
            var review = await _createReviewHandler.ExecuteAsync(new CreateReviewCommand
            (
                ProductId: productId,
                Rating: request.Rating,
                ReviewText: request.ReviewText
            ));
            return CreatedAtRoute("GetReviewById", new { reviewId = review.Id }, review);
        }

        [HttpPut("{productId:int}")]
        [Authorize(Policy = Permissions.Products.Update)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> Update(int productId, [FromForm] UpdateProductRequest request)
        {
            if (request.Images.Count != request.ImageOrders.Count) return BadRequest("Each image must have a corresponding image order.");

            ProductDto result = await _updateProductHandler.ExecuteAsync(new UpdateProductCommand
            {
                ProductId = productId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                MainImage = request.MainImage,
                CategoryId = request.CategoryId,
                Images = [.. request.Images.Select((image, index) => new UpdateProductImageCommand
                {
                    Image = image, ImageOrder = request.ImageOrders[index]
                })]
            });

            return Ok(result.WithFullImageUrls(_fileUrlGenerator));
        }
    }
}
