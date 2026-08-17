using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Order.Mappings;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Order
{
    public sealed class GetOrdersHandler
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PagedResult<OrderDto>> ExecuteAsync(GetOrdersQuery query)
        {
            if (query.PageNumber < 1)throw new DomainException("Page number must be greater than zero.");

            if (query.PageSize < 1 || query.PageSize > 100) throw new DomainException("Page size must be between 1 and 100.");

            var result = await _orderRepository.GetPagedAsync(query);

            return new PagedResult<OrderDto>
            {
                Items = [.. result.Items.Select(o => o.ToDto())],
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
