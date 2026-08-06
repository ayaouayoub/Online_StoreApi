namespace OnlineStore.Api.Controllers.Product.Requests
{
    public sealed record GetProductsRequest
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
