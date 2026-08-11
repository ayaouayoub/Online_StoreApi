using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Contracts.Payments;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Interfaces.Services.Payments
{
    public interface IPaymentGateway
    {
        PaymentProvider Provider { get; }
        Task<PaymentGatewayResult> PayAsync(PaymentRequest request);
    }
}
