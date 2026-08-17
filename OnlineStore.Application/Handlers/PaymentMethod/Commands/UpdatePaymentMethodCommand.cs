namespace OnlineStore.Application.Handlers.PaymentMethod.Commands
{
    public sealed record UpdatePaymentMethodCommand(int PaymentMethodId, string? Description);
}
