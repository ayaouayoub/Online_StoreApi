using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.Payment.Queries
{
    public sealed class GetPaymentsQuery
    {
        public int? OrderId { get; init; }
        public int? PaymentMethodId { get; init; }
        public PaymentStatus? Status { get; init; }
        public string? Currency { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
