namespace OnlineStore.Api.Controllers.Product.Requests
{
    public sealed record UpdateProductRequest
    {
        public string Name { get; init; } = null!;

        public string Description { get; init; } = null!;

        public decimal Price { get; init; }

        public IFormFile? MainImage { get; init; }

        public int CategoryId { get; init; }

        public List<IFormFile> Images { get; init; } = [];

        public List<short> ImageOrders { get; init; } = [];
    }
}
