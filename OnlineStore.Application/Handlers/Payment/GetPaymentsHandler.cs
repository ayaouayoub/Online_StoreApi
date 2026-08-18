using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Payment.Mappings;
using OnlineStore.Application.Handlers.Payment.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Payment
{
    public sealed class GetPaymentsHandler
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentsHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PagedResultDto<PaymentDto>> ExecuteAsync(GetPaymentsQuery query)
        {
            var result = await _paymentRepository.GetPagedAsync(query);

            return new PagedResultDto<PaymentDto>
            {
                Items = [.. result.Items.Select(x => x.ToDto())],
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}