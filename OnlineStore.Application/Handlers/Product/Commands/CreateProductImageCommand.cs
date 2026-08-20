using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineStore.Application.Handlers.Product.Commands
{
    public sealed record CreateProductImageCommand
    (
        IFormFile Image,
        short ImageOrder
    );
}
