using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.PaymentMethod.Mappings;
using OnlineStore.Application.Handlers.PaymentMethod.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.PaymentMethod
{
    public sealed class GetPaymentMethodHandler
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public GetPaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<PaymentMethodDto> ExecuteAsync(GetPaymentMethodQuery query)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(query.PaymentMethodId) ?? throw new NotFoundException("Payment method not found.");
            return paymentMethod.ToDto();
        }
    }
}