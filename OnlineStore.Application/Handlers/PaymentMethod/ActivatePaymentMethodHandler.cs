using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.PaymentMethod.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.PaymentMethod
{
    public sealed class ActivatePaymentMethodHandler
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public ActivatePaymentMethodHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task ExecuteAsync(ActivatePaymentMethodCommand command)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId) ?? throw new NotFoundException("Payment method not found.");

            paymentMethod.Activate();

            if (!await _paymentMethodRepository.ActivateAsync(command.PaymentMethodId)) throw new DomainException("Failed to activate payment method.");
        }
    }
}