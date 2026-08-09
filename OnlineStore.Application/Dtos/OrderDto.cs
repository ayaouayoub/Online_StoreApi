using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Dtos
{
    public class OrderDto
    {
        public int Id { get; init; }
        public int CustomerId { get; init; }
        public DateTime CreatedAt { get; init; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; init; }
        public IReadOnlyCollection<OrderItemDto> Items { get; init; } = [];
    }
}
