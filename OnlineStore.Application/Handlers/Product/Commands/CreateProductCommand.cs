using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineStore.Application.Handlers.Product.Commands
{
    public sealed record CreateProductCommand
    (
        string Name,
        string? Description,
        decimal Price,
        int QuantityInStock,
        IFormFile? MainImage,
        int CategoryId,
        IReadOnlyCollection<CreateProductImageCommand> Images
    );
}
