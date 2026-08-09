namespace OnlineStore.Application.Handlers.Customer.Models
{
    public sealed record CustomerDetails
    {
        public Domain.Entities.Customer Customer { get; init; } = null!;
        public int UserId { get; init; }
        public string Name { get; init; } = null!;
        public string Username { get; init; } = null!;
        public bool IsActive { get; init; }
    }
}
