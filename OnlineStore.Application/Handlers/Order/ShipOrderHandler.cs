using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Order.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Order
{
    public sealed class ShipOrderHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingRepository _shippingRepository;

        public ShipOrderHandler(IOrderRepository orderRepository, IShippingRepository shippingRepository)
        {
            _orderRepository = orderRepository;
            _shippingRepository = shippingRepository;
        }

        public async Task ExecuteAsync(ShipOrderCommand command)
        {
            if (command.OrderId <= 0) throw new DomainException("Invalid order id.");

            var order = await _orderRepository.GetByIdAsync(command.OrderId)  ?? throw new NotFoundException("Order not found.");

            order.MarkAsShipped();

            var shipping = await _shippingRepository.GetByOrderIdAsync(order.Id) ?? throw new NotFoundException("Shipping does not exist for this order.");

            shipping.MarkAsShipped();

            await _orderRepository.ShipAsync(order.Id);
        }
    }
}
