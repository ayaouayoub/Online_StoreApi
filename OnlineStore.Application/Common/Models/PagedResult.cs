using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Common.Models
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];

        public int Page { get; init; }

        public int PageSize { get; init; }

        public int TotalCount { get; init; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
