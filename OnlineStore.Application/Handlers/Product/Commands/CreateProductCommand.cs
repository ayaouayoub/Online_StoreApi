using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Product.Commands
{
    public sealed record CreateProductCommand
    (
        string Name,
        string? Description,
        decimal Price,
        int QuantityInStock,
        string? MainImageUrl,
        int CategoryId,
        IReadOnlyCollection<CreateProductImageCommand> Images
    );
}
