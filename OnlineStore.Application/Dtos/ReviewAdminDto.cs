namespace OnlineStore.Application.Dtos
{
    public class ReviewAdminDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = null!;
        public int CustomerId { get; init; }
        public string CustomerName { get; init; } = null!;
        public string? ReviewText { get; init; }
        public decimal Rating { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? DeletedAt { get; init; }
        public int? DeletedById { get; init; }
        public string? DeletedByName { get; init; }
        public string? DeleteReason { get; init; }
    }
}
