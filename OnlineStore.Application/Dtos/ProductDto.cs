using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Dtos
{
    public sealed record ProductDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public int QuantityInStock { get; init; }
        public string? MainImageUrl { get; set; }
        public IReadOnlyCollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public int CategoryId { get; init; }
        public CategoryDto Category { get; init; } = null!;
    }
}
