using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Dtos
{
    public sealed record PaymentDto
    {
        public int Id { get; init; }
        public int OrderId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = null!;
        public int PaymentMethodId { get; init; }
        public PaymentMethodDto PaymentMethod { get; init; } = null!;
        public DateTime TransactionDate { get; init; }
        public PaymentStatus Status { get; init; }
        public string? TransactionId { get; init; }
    }
}
