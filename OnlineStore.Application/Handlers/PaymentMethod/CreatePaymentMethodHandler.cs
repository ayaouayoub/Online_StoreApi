using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.PaymentMethod.Commands;
using OnlineStore.Application.Handlers.PaymentMethod.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.PaymentMethod
{
    public sealed class CreatePaymentMethodHandler
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public CreatePaymentMethodHandler(
            IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<PaymentMethodDto> ExecuteAsync(CreatePaymentMethodCommand command)
        {
            var paymentMethod = Domain.Entities.PaymentMethod.Create(command.Name, command.Description);

            var createdPaymentMethod = await _paymentMethodRepository.CreateAsync(paymentMethod);

            return createdPaymentMethod.ToDto();
        }
    }
}