using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Dtos
{
    public sealed record ShippingDto
    (
        int Id,
        int OrderId,
        string CarrierName,
        string TrackingNumber,
        ShippingStatus Status,
        DateTime EstimatedDeliveryDate,
        DateTime? ActualDeliveryDate
    );
}
