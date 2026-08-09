namespace OnlineStore.Api.Controllers.Order.Requests
{
    public sealed record CreateOrderRequest
    {
        public IReadOnlyCollection<CreateOrderItemRequest> Items { get; init; } = [];
    }
}
