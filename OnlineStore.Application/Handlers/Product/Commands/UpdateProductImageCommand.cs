using Microsoft.AspNetCore.Http;

namespace OnlineStore.Application.Handlers.Product.Commands
{
    public sealed record UpdateProductImageCommand
    {
        public IFormFile Image { get; init; } = null!;

        public short ImageOrder { get; init; }
    }
}
