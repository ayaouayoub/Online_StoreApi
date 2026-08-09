using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Dtos
{
    public class OrderItemDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = null!;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
    }
}
