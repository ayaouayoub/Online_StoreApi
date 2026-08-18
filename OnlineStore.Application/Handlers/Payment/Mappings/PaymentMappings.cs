using OnlineStore.Application.Dtos;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Handlers.Payment.Mappings
{
    public static class PaymentMappings
    {
        public static PaymentDto ToDto(this Domain.Entities.Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Money.Amount,
                Currency = payment.Money.Currency.Code,
                PaymentMethodId = payment.PaymentMethodId,
                PaymentMethod = payment.PaymentMethod!.ToDto(),
                TransactionDate = payment.TransactionDate,
                Status = payment.Status,
                TransactionId = payment.TransactionId
            };
        }

        public static PaymentMethodDto ToDto(this Domain.Entities.PaymentMethod paymentMethod)
        {
            return new PaymentMethodDto
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.Name,
                Description = paymentMethod.Description,
                IsActive = paymentMethod.IsActive,
                CreatedAt = paymentMethod.CreatedAt
            };
        }
    }
}