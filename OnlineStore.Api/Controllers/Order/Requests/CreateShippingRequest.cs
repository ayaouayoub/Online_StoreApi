namespace OnlineStore.Api.Controllers.Order.Requests
{
    public sealed record CreateShippingRequest
    {
        public string CarrierName { get; init; } = null!;
        public string TrackingNumber { get; init; } = null!;
        public DateTime EstimatedDeliveryDate { get; init; }
    }
}
