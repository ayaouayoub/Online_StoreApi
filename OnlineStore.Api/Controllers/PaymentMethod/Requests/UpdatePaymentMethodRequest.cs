namespace OnlineStore.Api.Controllers.PaymentMethod.Requests
{
    public sealed record UpdatePaymentMethodRequest
    {
        public string? Description { get; init; }
    }
}
