namespace OnlineStore.Application.Handlers.User.Commands
{
    public sealed record ChangeMyPasswordCommand(string CurrentPassword, string NewPassword);
}
