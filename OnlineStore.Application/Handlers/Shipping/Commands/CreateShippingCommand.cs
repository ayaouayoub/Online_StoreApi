namespace OnlineStore.Application.Handlers.Shipping.Commands
{
    public sealed record CreateShippingCommand
    (
        int OrderId,
        string CarrierName,
        string TrackingNumber,
        DateTime EstimatedDeliveryDate
    );
}
