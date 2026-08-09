using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; }
        public int ProductId { get; }
        public Product? Product { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public string ProductName { get; } = null!;
        public decimal TotalPrice => UnitPrice * Quantity;

        private OrderItem(int id, int productId, Product? product, int quantity, decimal unitPrice, string productName)
        {
            if (product is not null && product.Id != productId) throw new DomainException("Product id mismatch.");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
            if (unitPrice <= 0) throw new DomainException("UnitPrice cannot be nigative.");

            Id = id;
            ProductId = productId;
            Product = product;
            Quantity = quantity;
            UnitPrice = unitPrice;
            ProductName = productName;
        }

        public static OrderItem Create(Product product, int quantity) 
        {
            if (product == null) throw new DomainException("Product cannot be null.");
            return new OrderItem(-1, product.Id, product, quantity, product.Price, product.Name);
        }

        public static OrderItem Load(int id, int productId, int quantity, decimal unitPrice, string productName)
        {
            return new OrderItem(id, productId, null, quantity, unitPrice, productName);
        }

        public static OrderItem LoadWithProduct(int id, Product product, int quantity, decimal unitPrice, string productName)
        {
            return new OrderItem(id, product.Id, product, quantity, unitPrice, productName);
        }
    }
}
