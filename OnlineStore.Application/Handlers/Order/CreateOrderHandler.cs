using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Order.Commands;
using OnlineStore.Application.Handlers.Order.Mappings;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Order
{
    public sealed class CreateOrderHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUser _currentUser;

        public CreateOrderHandler(ICustomerRepository customerRepository, IProductRepository productRepository, IOrderRepository orderRepository, ICurrentUser currentUser)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _currentUser = currentUser;
        }

        public async Task<OrderDto> ExecuteAsync(CreateOrderCommand command)
        {
            var result = await _customerRepository.GetByUserIdAsync(_currentUser.UserId) ?? throw new ForbiddenException("Only customers can create orders.");

            var orderItems = new List<OrderItem>();

            if (!command.Items.Any()) throw new DomainException("Order must contain at least one item.");

            foreach (var item in command.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId) ?? throw new NotFoundException($"Product {item.ProductId} not found.");
                if (item.Quantity > product.QuantityInStock) throw new DomainException($"Insufficient stock for product {item.ProductId}.");
                orderItems.Add(OrderItem.Create(product, item.Quantity));
            }

            var order = Domain.Entities.Order.Create(result.Customer, orderItems);

            int orderId = await _orderRepository.CreateAsync(order);

            return order.ToDto(orderId);
        }
    }
}
