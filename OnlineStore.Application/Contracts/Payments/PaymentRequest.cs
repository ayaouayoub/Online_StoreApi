using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Contracts.Payments
{
    public sealed record PaymentRequest
    {
        public int OrderId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "USD";
    }
}
