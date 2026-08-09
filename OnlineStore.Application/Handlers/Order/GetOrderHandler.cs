using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Order.Mappings;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Order
{
    public sealed class GetOrderHandler
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> ExecuteAsync(GetOrderQuery query)
        {
            var order = await _orderRepository.GetByIdAsync(query.OrderId) ?? throw new NotFoundException($"Order {query.OrderId} not found.");
            return order.ToDto();
        }
    }
}
