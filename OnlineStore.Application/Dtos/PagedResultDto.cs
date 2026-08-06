using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Dtos
{
    public sealed class PagedResultDto<T>
    {
        public IReadOnlyCollection<T> Items { get; init; } = [];
        public int TotalCount { get; init; }

        public int Page { get; init; }

        public int PageSize { get; init; }
    }
}
