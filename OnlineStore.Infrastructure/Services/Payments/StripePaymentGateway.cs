using OnlineStore.Application.Contracts.Payments;
using OnlineStore.Application.Interfaces.Services.Payments;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Services.Payments
{
    public class StripePaymentGateway : IPaymentGateway
    {
        public PaymentProvider Provider => PaymentProvider.Stripe;


        public async Task<PaymentGatewayResult> PayAsync(PaymentRequest request)
        {
            await Task.Delay(100);
            return PaymentGatewayResult.Succeeded($"STRIPE-{Guid.NewGuid()}");
        }
    }
}
