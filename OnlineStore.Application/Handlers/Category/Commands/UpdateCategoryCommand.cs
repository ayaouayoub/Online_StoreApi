using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Category.Commands
{
    public sealed record UpdateCategoryCommand
    {
        public int CategoryId { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public int? ParentCategoryId { get; init; }
        public int DisplayOrder { get; init; }
    }
}
