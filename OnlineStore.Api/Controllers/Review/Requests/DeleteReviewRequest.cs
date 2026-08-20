namespace OnlineStore.Api.Controllers.Review.Requests
{
    public sealed record DeleteReviewRequest
    {
        public string? DeleteReason { get; init; }
    }
}
