namespace OnlineStore.Application.Handlers.Review.Commands
{
    public sealed record CreateReviewCommand
    (
        int ProductId,
        decimal Rating,
        string? ReviewText
    );
}
