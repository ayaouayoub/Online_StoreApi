using System;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; }
        public string Url { get; private set; } = null!;
        public short ImageOrder {  get; private set; }

        private ProductImage(int id, string url, short imageOrder)
        {
            _ValidateUrl(url);
            _ValidateOrder(imageOrder);
            Id = id;
            Url = url;
            ImageOrder = imageOrder;
        }

        public static ProductImage Create(string url, short imageOrder)
        {
            return new ProductImage(-1, url, imageOrder);
        }

        public static ProductImage Load(int id, string url, short imageOrder)
        {
            return new ProductImage(id, url, imageOrder);
        }

        public void ChangeUrl(string url)
        {
            if (Url == url) return;
            _ValidateUrl(url);
            Url = url;
        }

        public void ChangeOrder(short order)
        {
            if (ImageOrder == order) return;
            _ValidateOrder(order);
            ImageOrder = order;
        }

        private static void _ValidateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new DomainException("Url cannot be null or empty");
        }

        private static void _ValidateOrder(short order)
        {
            if (order < 0) throw new DomainException("order number cannot be negative.");
        }
    }
}
