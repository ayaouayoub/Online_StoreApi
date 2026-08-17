namespace OnlineStore.Application.Handlers.PaymentMethod.Commands
{
    public sealed record CreatePaymentMethodCommand(string Name, string? Description);
}
