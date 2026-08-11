using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Order.Mappings;
using OnlineStore.Application.Handlers.Order.Queries;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.Order
{
    public sealed class GetOrderHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICustomerRepository _customerRepository;

        public GetOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> ExecuteAsync(GetOrderQuery query)
        {
            var order = await _orderRepository.GetByIdAsync(query.OrderId) ?? throw new NotFoundException($"Order {query.OrderId} not found.");
            var user = _currentUser.User;
            if (user.RoleId == (int)RoleType.Customer)
            {
                var details = await _customerRepository.GetByUserIdAsync(user.Id) ?? throw new ForbiddenException("Customer not found.");
                if (order.CustomerId != details.Customer.Id) throw new ForbiddenException("You cannot access this order.");
            }
            return order.ToDto();
        }
    }
}
