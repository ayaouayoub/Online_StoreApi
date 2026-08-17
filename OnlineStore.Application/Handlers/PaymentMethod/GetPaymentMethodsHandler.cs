using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.PaymentMethod.Mappings;
using OnlineStore.Application.Handlers.PaymentMethod.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.PaymentMethod
{
    public sealed class GetPaymentMethodsHandler
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public GetPaymentMethodsHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<IEnumerable<PaymentMethodDto>> ExecuteAsync(GetPaymentMethodsQuery query)
        {
            var paymentMethods =await _paymentMethodRepository.GetAllAsync();
            return [.. paymentMethods.Select(x => x.ToDto())];
        }
    }
}