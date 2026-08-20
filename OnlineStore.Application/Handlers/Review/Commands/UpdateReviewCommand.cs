namespace OnlineStore.Application.Handlers.Review.Commands
{
    public sealed record UpdateReviewCommand
    (
        int ReviewId,
        decimal Rating,
        string? ReviewText
    );
}
