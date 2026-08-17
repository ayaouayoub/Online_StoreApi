using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.PaymentMethod.Commands;
using OnlineStore.Application.Handlers.PaymentMethod.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.PaymentMethod
{
    public sealed class UpdatePaymentMethodHandler
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public UpdatePaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<PaymentMethodDto> ExecuteAsync(UpdatePaymentMethodCommand command)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId) ?? throw new NotFoundException("Payment method not found.");

            paymentMethod.ChangeDescription(command.Description);

            if (!await _paymentMethodRepository.UpdateAsync(paymentMethod)) throw new DomainException("Failed to update payment method.");

            return paymentMethod.ToDto();
        }
    }
}