using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; }
        public int ProductId { get; }
        public Product Product { get; } = null!;
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public decimal TotalPrice => UnitPrice * Quantity;

        private OrderItem(int id, int productId, Product product, int quantity, decimal unitPrice)
        {
            if (product == null) throw new DomainException("Product cannot be null.");
            if (product.Id != productId) throw new DomainException("Product id mismatch.");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice <= 0) throw new DomainException("UnitPrice cannot be nigative.");

            Id = id;
            ProductId = productId;
            Product = product;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public static OrderItem Create(Product product, int quantity) 
        {
            return new OrderItem(-1, product.Id, product, quantity, product.Price);
        }

        public static OrderItem Load(int id, Product product, int quantity, decimal unitPrice)
        {
            return new OrderItem(id, product.Id, product, quantity, unitPrice);
        }
    }
}
