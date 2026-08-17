using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.PaymentMethod.Mappings
{
    public static class PaymentMethodMappings
    {
        public static PaymentMethodDto ToDto(this Domain.Entities.PaymentMethod paymentMethod)
        {
            return new PaymentMethodDto
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.Name,
                IsActive = paymentMethod.IsActive,
                Description = paymentMethod.Description,
                CreatedAt = paymentMethod.CreatedAt
            };
        }
    }
}
