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
    public sealed class DeliverOrderHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingRepository _shippingRepository;

        public DeliverOrderHandler(IOrderRepository orderRepository, IShippingRepository shippingRepository)
        {
            _orderRepository = orderRepository;
            _shippingRepository = shippingRepository;
        }

        public async Task ExecuteAsync(DeliverOrderCommand command)
        {
            if (command.OrderId <= 0) throw new DomainException("Invalid order id.");

            var order = await _orderRepository.GetByIdAsync(command.OrderId) ?? throw new NotFoundException("Order not found.");

            order.MarkAsDelivered();

            var shipping = await _shippingRepository.GetByOrderIdAsync(order.Id) ?? throw new NotFoundException("Shipping does not exist for this order.");

            shipping.MarkAsDelivered();

            await _orderRepository.DeliverAsync(order.Id);
        }
    }
}
