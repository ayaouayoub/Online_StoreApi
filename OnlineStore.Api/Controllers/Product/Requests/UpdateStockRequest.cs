namespace OnlineStore.Api.Controllers.Product.Requests
{
    public sealed record UpdateStockRequest
    {
        public int QuantityChange { get; init; }
    }
}
