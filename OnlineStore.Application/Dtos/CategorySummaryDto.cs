using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Dtos
{
    public sealed record CategorySummaryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public LookupDto? Parent { get; init; }
    }
}
