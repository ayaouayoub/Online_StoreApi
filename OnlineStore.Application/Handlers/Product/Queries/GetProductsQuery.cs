using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Product.Queries
{
    public sealed record GetProductsQuery
    (
        int Page = 1,
        int PageSize = 20,
        string? Search = null,
        int? CategoryId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string? SortBy = null,
        bool Descending = false
    );
}
