using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Dtos
{
    public class ReviewDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = null!;
        public int CustomerId { get; init; }
        public string CustomerName { get; init; } = null!;
        public string? ReviewText { get; init; }
        public decimal Rating { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
