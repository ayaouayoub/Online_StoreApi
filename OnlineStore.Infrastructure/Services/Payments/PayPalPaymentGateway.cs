using OnlineStore.Application.Contracts.Payments;
using OnlineStore.Application.Interfaces.Services.Payments;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Services.Payments
{
    public class PayPalPaymentGateway : IPaymentGateway
    {
        public PaymentProvider Provider => PaymentProvider.PayPal;

        public async Task<PaymentGatewayResult> PayAsync(PaymentRequest request)
        {
            await Task.Delay(100);
            return PaymentGatewayResult.Succeeded($"PAYPAL-{Guid.NewGuid()}");
        }
    }
}
