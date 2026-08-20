namespace OnlineStore.Api.Controllers.Customer.Requests
{
    public sealed record UpdateReviewRequest
    {
        public decimal Rating { get; init; }

        public string? ReviewText { get; init; }
    }
}
