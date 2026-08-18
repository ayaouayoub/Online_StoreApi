namespace OnlineStore.Api.Controllers.Customer.Requests
{
    public sealed record UpdateCustomerRequest
    {
        public string Email { get; init; } = null!;
        public string? Phone { get; init; }
        public string Address { get; init; } = null!;
    }
}
