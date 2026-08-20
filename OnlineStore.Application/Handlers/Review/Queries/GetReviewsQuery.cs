namespace OnlineStore.Application.Handlers.Review.Queries
{
    public sealed record GetReviewsQuery
    (
        int? ReviewId = null,
        int? ProductId = null,
        int? CustomerId = null,
        decimal? Rating = null,
        string? ReviewText = null,
        DateTime? CreatedFrom = null,
        DateTime? CreatedTo = null,
        DateTime? UpdatedFrom = null,
        DateTime? UpdatedTo = null,
        bool? IsDeleted = null,
        DateTime? DeletedFrom = null,
        DateTime? DeletedTo = null,
        int? DeletedBy = null,
        string? DeleteReason = null,
        int Page = 1,
        int PageSize = 20
    );
}
