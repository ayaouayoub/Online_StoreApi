using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = [];
        public int Id { get; }
        public int CustomerId { get; }
        public Customer? Customer { get; }
        public DateTime CreatedAt { get; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount => _items.Sum(i => i.TotalPrice);
        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order(int id, int customerId, Customer? customer, OrderStatus status, DateTime? createdAt = null)
        {
            if (customer is not null && customerId != customer.Id) throw new DomainException("Customer id mismatch");
            Id = id;
            CustomerId = customerId;
            Customer = customer;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            Status = status;
        }

        public static Order Create(Customer customer, IEnumerable<OrderItem> Items)
        {
            ArgumentNullException.ThrowIfNull(customer);
            var order = new Order(-1, customer.Id, customer, OrderStatus.PendingPayment);
            foreach(var item in Items) order.AddItem(item);
            return order;
        }

        public static Order Load(int id, OrderStatus status, DateTime createdAt, int customerId, Customer? customer = null)
        {
            return new Order(id, customerId, customer, status, createdAt);
        }

        public void AddItem(OrderItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (_items.Any(i => i.ProductId == item.ProductId)) throw new DomainException("Product already exists in order.");
            if (Items.Count >= 100) throw new DomainException("The limit for the number of items has been exceeded.");
            _items.Add(item);
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.PendingPayment) throw new DomainException("Only pending orders can be marked as paid.");
            Status = OrderStatus.Paid;
        }

        public void StartProcessing()
        {
            if (Status != OrderStatus.Paid) throw new DomainException("Only paid orders can be processed.");
            Status = OrderStatus.Processing;
        }

        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Processing) throw new DomainException("Only processing orders can be shipped.");
            Status = OrderStatus.Shipped;
        }

        public void MarkAsDelivered()
        {
            if (Status != OrderStatus.Shipped) throw new DomainException("Only shipped orders can be delivered.");
            Status = OrderStatus.Delivered;
        }
    }
}
