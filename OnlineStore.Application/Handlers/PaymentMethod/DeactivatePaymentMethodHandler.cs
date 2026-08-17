using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.PaymentMethod.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.PaymentMethod
{
    public sealed class DeactivatePaymentMethodHandler
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public DeactivatePaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task ExecuteAsync(DeactivatePaymentMethodCommand command)
        {
            var paymentMethod =await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId) ?? throw new NotFoundException("Payment method not found.");

            paymentMethod.Deactivate();

            if (!await _paymentMethodRepository.DeactivateAsync(command.PaymentMethodId)) throw new DomainException("Failed to deactivate payment method.");
        }
    }
}