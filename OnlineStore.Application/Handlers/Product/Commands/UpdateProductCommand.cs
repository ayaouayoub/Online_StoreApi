using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineStore.Application.Handlers.Product.Commands
{
    public sealed record UpdateProductCommand
    {
        public int ProductId { get; init; }

        public string Name { get; init; } = null!;

        public string Description { get; init; } = null!;

        public decimal Price { get; init; }

        public IFormFile? MainImage { get; init; }

        public int CategoryId { get; init; }

        public IReadOnlyCollection<UpdateProductImageCommand> Images { get; init; } = [];
    }
}
