namespace OnlineStore.Api.Controllers.Product.Requests
{
    public sealed class CreateProductRequest
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public int QuantityInStock { get; init; }
        public int CategoryId { get; init; }

        public IFormFile? MainImage { get; init; }

        public IFormFile[] Images { get; init; } = [];
    }
}
