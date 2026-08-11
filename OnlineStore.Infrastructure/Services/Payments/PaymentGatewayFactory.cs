using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Interfaces.Services.Payments;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Services.Payments
{
    public sealed class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;

        public PaymentGatewayFactory(IEnumerable<IPaymentGateway> gateways)
        {
            _gateways = gateways;
        }

        public IPaymentGateway Get(PaymentProvider provider)
        {
            return _gateways.FirstOrDefault(x => x.Provider == provider) ?? throw new NotSupportedException($"Payment provider '{provider}' is not supported.");
        }
    }
}
