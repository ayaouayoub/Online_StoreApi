namespace OnlineStore.Application.Contracts.Payments
{
    public class PaymentGatewayResult
    {
        public bool Success { get; init; }
        public string? TransactionId { get; init; }
        public string? ErrorMessage { get; init; }

        public static PaymentGatewayResult Succeeded(string transactionId)
        {
            return new PaymentGatewayResult
            {
                Success = true,
                TransactionId = transactionId
            };
        }

        public static PaymentGatewayResult Failed(string errorMessage)
        {
            return new PaymentGatewayResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
