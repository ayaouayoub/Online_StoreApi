using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Shipping.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Application.Handlers.Shipping.Mappings;

namespace OnlineStore.Application.Handlers.Shipping
{
    public sealed class CreateShippingHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingRepository _shippingRepository;

        public CreateShippingHandler(IOrderRepository orderRepository, IShippingRepository shippingRepository)
        {
            _orderRepository = orderRepository;
            _shippingRepository = shippingRepository;
        }

        public async Task<ShippingDto> ExecuteAsync(CreateShippingCommand command)
        {
            var order = await _orderRepository.GetByIdAsync(command.OrderId) ?? throw new NotFoundException("Order not found.");

            if (order.Status != OrderStatus.Paid) throw new DomainException("Only paid orders can have shipping created.");

            var existingShipping = await _shippingRepository.GetByOrderIdAsync(order.Id);

            if (existingShipping is not null) throw new DomainException("Shipping already exists for this order.");

            var shipping = Domain.Entities.Shipping.Create
            (
                order: order,
                carrierName: command.CarrierName.Trim(),
                trackingNumber: command.TrackingNumber.Trim(),
                estimatedDeliveryDate: command.EstimatedDeliveryDate
            );

            var createdShipping = await _shippingRepository.CreateAsync(shipping) ?? throw new DomainException("Failed to create shipping.");

            return createdShipping.ToDto();
        }
    }
}
