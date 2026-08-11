using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Interfaces.Services.Payments
{
    public interface IPaymentGatewayFactory
    {
        IPaymentGateway Get(PaymentProvider provider);
    }
}
