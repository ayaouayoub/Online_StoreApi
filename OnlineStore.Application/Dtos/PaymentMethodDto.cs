namespace OnlineStore.Application.Dtos
{
    public sealed record PaymentMethodDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
