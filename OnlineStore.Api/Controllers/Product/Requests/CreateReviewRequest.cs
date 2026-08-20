namespace OnlineStore.Api.Controllers.Product.Requests
{
    public sealed class CreateReviewRequest
    {
        public decimal Rating { get; init; }

        public string? ReviewText { get; init; }
    }
}
