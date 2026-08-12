using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Shipping.Mappings;
using OnlineStore.Application.Handlers.Shipping.Queries;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.Shipping
{
    public sealed class GetOrderShippingHandler
    {
        private readonly IShippingRepository _shippingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IOrderRepository _orderRepository;

        public GetOrderShippingHandler(IShippingRepository shippingRepository, ICustomerRepository customerRepository, ICurrentUser currentUser, IOrderRepository orderRepository)
        {
            _shippingRepository = shippingRepository;
            _currentUser = currentUser;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ShippingDto> ExecuteAsync(GetOrderShippingQuery query)
        {
            var shipping = await _shippingRepository.GetByOrderIdAsync(query.OrderId) ?? throw new NotFoundException("No shipping for this order.");

            var user = _currentUser.User;

            if (user.RoleId == (int)RoleType.Customer)
            {
                var details = await _customerRepository.GetByUserIdAsync(user.Id) ?? throw new ForbiddenException("Customer not found.");
                var order = await _orderRepository.GetByIdAsync(query.OrderId) ?? throw new NotFoundException("Order not found.");
                if (order.CustomerId != details.Customer.Id) throw new ForbiddenException("You cannot access this shippment.");
            }

            return shipping.ToDto();
        }
    }
}
