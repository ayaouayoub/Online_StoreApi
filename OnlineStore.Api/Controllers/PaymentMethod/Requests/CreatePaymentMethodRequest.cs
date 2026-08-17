namespace OnlineStore.Api.Controllers.PaymentMethod.Requests
{
    public sealed record CreatePaymentMethodRequest
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }
}
