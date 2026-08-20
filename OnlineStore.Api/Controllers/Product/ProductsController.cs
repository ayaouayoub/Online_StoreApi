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
using Microsoft.AspNetCore.Mvc.Formatters;
using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Application.Interfaces;
using OnlineStore.Infrastructure.Authorization;

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
        private readonly GetProductsHandler _getProductsHandler;
        private readonly UpdateStockHandler _updateStockHandler;
        private readonly GetReviewsByProductIdHandler _getReviewsByProductIdHandler;
        private readonly ICurrentUser _currentUser;
        private readonly CreateReviewHandler _createReviewHandler;

        public ProductsController(CreateProductHandler createProductHandler, IImageStorageService imageStorageService, FileUrlGenerator fileUrlGenerator, GetProductHandler getProductHandler, GetProductsHandler getProductsHandler, UpdateStockHandler updateStockHandler, GetReviewsByProductIdHandler getReviewsByProductIdHandler, ICurrentUser currentUser, CreateReviewHandler createReviewHandler)
        {
            _createProductHandler = createProductHandler;
            _imageStorageService = imageStorageService;
            _fileUrlGenerator = fileUrlGenerator;
            _getProductHandler = getProductHandler;
            _getProductsHandler = getProductsHandler;
            _updateStockHandler = updateStockHandler;
            _getReviewsByProductIdHandler = getReviewsByProductIdHandler;
            _currentUser = currentUser;
            _createReviewHandler = createReviewHandler;
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

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts([FromQuery] GetProductsRequest request)
        {
            var resultDto = await _getProductsHandler.ExecuteAsync(new GetProductsQuery
            {
                CategoryId = request.CategoryId,
                Descending = request.Descending,
                MaxPrice = request.MaxPrice,
                MinPrice = request.MinPrice,
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                SortBy = request.SortBy
            });

            foreach (ProductDto item in resultDto.Items)
            {
                item.MainImageUrl = _fileUrlGenerator.GetUrl(item.MainImageUrl);
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
    }
}
