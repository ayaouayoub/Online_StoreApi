namespace OnlineStore.Api.Controllers.Product.Requests
{
    public sealed record UpdateProductImageRequest
    {
        public IFormFile Image { get; init; } = null!;
        public short ImageOrder { get; init; }
    }
}
