namespace OnlineStore.Application.Handlers.Review.Commands
{
    public sealed record DeleteMyReviewCommand
    (
        int ReviewId,
        string? DeleteReason
    );
}
