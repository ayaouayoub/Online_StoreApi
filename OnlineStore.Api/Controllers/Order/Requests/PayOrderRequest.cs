using OnlineStore.Domain.Enums;

namespace OnlineStore.Api.Controllers.Order.Requests
{
    public sealed record PayOrderRequest 
    { 
        public int PaymentMethodId { get; init; } 
        public PaymentProvider Provider { get; init; } 
    }
}
