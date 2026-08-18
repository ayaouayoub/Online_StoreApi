using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Payment.Mappings;
using OnlineStore.Application.Handlers.Payment.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Payment
{
    public sealed class GetPaymentHandler
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentDto> ExecuteAsync(GetPaymentQuery query)
        {
            var payment = await _paymentRepository.GetByIdAsync(query.PaymentId) ?? throw new NotFoundException("Payment not found.");
            return payment.ToDto();
        }
    }
}