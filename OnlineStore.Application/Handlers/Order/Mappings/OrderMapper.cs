using OnlineStore.Application.Dtos;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Handlers.Order.Mappings
{
    public static class OrderMapper
    {
        public static OrderDto ToDto(this Domain.Entities.Order order, int orderId)
        {
            return new OrderDto
            {
                Id = orderId,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = [.. order.Items.Select(ToDto)]
            };
        }

        public static OrderDto ToDto(this Domain.Entities.Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = [.. order.Items.Select(ToDto)]
            };
        }

        private static OrderItemDto ToDto(OrderItem item)
        {
            return new OrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            };
        }
    }
}
