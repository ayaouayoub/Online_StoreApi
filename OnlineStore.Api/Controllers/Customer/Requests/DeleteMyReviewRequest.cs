namespace OnlineStore.Api.Controllers.Customer.Requests
{
    public sealed record DeleteMyReviewRequest
    {
        public string? DeleteReason { get; init; }
    }
}
