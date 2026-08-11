namespace OnlineStore.Domain.ValueObjs
{
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        public Money(decimal amount, Currency currency)
        {
            if (amount < 0) throw new ArgumentException("Money amount cannot be negative.");
            Amount = amount;
            Currency = currency;
        }

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency.Equals(other.Currency);
        }

        public override bool Equals(object? obj) => Equals(obj as Money);

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    }
}
