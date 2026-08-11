namespace OnlineStore.Domain.ValueObjs
{
    public sealed class Currency : IEquatable<Currency>
    {
        public string Code { get; }
        public int FractionDigits { get; }

        private static readonly string[] _validCodes = { "USD", "EUR", "GBP", "JPY" };
        private static readonly int[] _fractionDigits = { 2, 2, 2, 0 };

        public Currency(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Currency code cannot be null or empty.");

            Code = code.ToUpperInvariant();

            var index = Array.IndexOf(_validCodes, Code);
            if (index == -1) throw new ArgumentException($"Invalid currency code: {Code}.");

            FractionDigits = _fractionDigits[index];
        }

        public bool Equals(Currency? other)
        {
            if (other is null) return false;
            return Code == other.Code;
        }
        public override bool Equals(object? obj) => Equals(obj as Currency);
        public override int GetHashCode() => Code.GetHashCode();
    }
}
