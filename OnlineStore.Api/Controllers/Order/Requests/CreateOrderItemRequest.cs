namespace OnlineStore.Api.Controllers.Order.Requests
{
    public sealed record CreateOrderItemRequest
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
