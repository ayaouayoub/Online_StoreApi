using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.ValueObjs;

namespace OnlineStore.Domain.Entities
{
    public class Payment
    {
        public int Id { get; }
        public int OrderId { get; }
        public int PaymentMethodId { get; }
        public PaymentMethod? PaymentMethod { get; }
        public DateTime TransactionDate { get; }
        public Money Money { get; }
        public string? TransactionId { get; private set; }
        public PaymentStatus Status { get; private set; }

        private Payment(int id,int orderId,int paymentMethodId,PaymentMethod? paymentMethod, Money money, PaymentStatus status, string? transactionId, DateTime transactionDate)
        {
            if (orderId <= 0) throw new DomainException("Invalid order.");

            if (paymentMethodId <= 0) throw new DomainException("Invalid payment method.");

            ArgumentNullException.ThrowIfNull(money);

            if (money.Amount <= 0)throw new DomainException("Payment amount must be greater than zero.");

            if (paymentMethod is not null && paymentMethodId != paymentMethod.Id) throw new DomainException("Payment method id mismatch.");

            if (status == PaymentStatus.Succeeded && string.IsNullOrWhiteSpace(transactionId)) throw new DomainException("Succeeded payment must have a transaction id.");

            Id = id;
            OrderId = orderId;
            PaymentMethodId = paymentMethodId;
            PaymentMethod = paymentMethod;
            Money = money;
            Status = status;
            TransactionId = transactionId;
            TransactionDate = transactionDate;
        }

        public static Payment Create(int orderId, Money money, PaymentMethod paymentMethod)
        {
            ArgumentNullException.ThrowIfNull(paymentMethod);

            if (!paymentMethod.IsActive) throw new DomainException("Payment method is inactive.");

            return new Payment
            (
                id: -1,
                orderId: orderId,
                paymentMethodId: paymentMethod.Id,
                paymentMethod: paymentMethod,
                money: money,
                status: PaymentStatus.Pending,
                transactionId: null,
                transactionDate: DateTime.UtcNow
            );
        }

        public static Payment Load(int id, int orderId, Money money, int paymentMethodId, PaymentStatus status, DateTime transactionDate, string? transactionId = null, PaymentMethod? paymentMethod = null)
        {
            return new Payment
            (
                id,
                orderId,
                paymentMethodId,
                paymentMethod,
                money,
                status,
                transactionId,
                transactionDate
            );
        }

        public void MarkAsSucceeded(string transactionId)
        {
            if (Status == PaymentStatus.Failed) throw new DomainException("Cannot mark failed payment as succeeded.");
            if (Status == PaymentStatus.Succeeded) return;
            if (string.IsNullOrWhiteSpace(transactionId)) throw new DomainException("Transaction ID is required.");
            TransactionId = transactionId;
            Status = PaymentStatus.Succeeded;
        }

        public void MarkAsFailed()
        {
            if (Status == PaymentStatus.Succeeded) throw new DomainException("Cannot mark succeeded payment as failed.");
            if (Status == PaymentStatus.Failed) return;
            Status = PaymentStatus.Failed;
        }
    }
}
