using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class Shipping
    {
        public int Id { get; }
        public int OrderId { get; }
        public Order? Order { get; }
        public string CarrierName { get; }
        public string TrackingNumber { get; }
        public ShippingStatus Status { get; private set; }
        public DateTime EstimatedDeliveryDate { get; }
        public DateTime? ActualDeliveryDate { get; private set; }

        private Shipping(int id, int orderId, Order? order, string carrierName, string trackingNumber, ShippingStatus status, DateTime estimatedDeliveryDate, DateTime? actualDeliveryDate)
        {
            if (orderId <= 0) throw new DomainException("Invalid order.");

            if (string.IsNullOrWhiteSpace(carrierName)) throw new DomainException("Carrier name is required.");

            if (carrierName.Length > 100) throw new DomainException("Carrier name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(trackingNumber)) throw new DomainException("Tracking number is required.");

            if (trackingNumber.Length > 50) throw new DomainException("Tracking number cannot exceed 50 characters.");

            if (status == ShippingStatus.Delivered && actualDeliveryDate is null) throw new DomainException("Delivered shipping must have an actual delivery date.");

            Id = id;
            OrderId = orderId;
            CarrierName = carrierName;
            TrackingNumber = trackingNumber;
            Status = status;
            EstimatedDeliveryDate = estimatedDeliveryDate;
            ActualDeliveryDate = actualDeliveryDate;
            Order = order;
        }

        public static Shipping Create(Order order, string carrierName, string trackingNumber, DateTime estimatedDeliveryDate)
        {
            if (estimatedDeliveryDate <= DateTime.UtcNow) throw new DomainException("Estimated delivery date must be in the future.");

            return new Shipping
            (
                id: -1,
                order.Id,
                order,
                carrierName,
                trackingNumber,
                ShippingStatus.Preparing,
                estimatedDeliveryDate,
                null
            );
        }

        public static Shipping Load(int id, int orderId, string carrierName, string trackingNumber, ShippingStatus status, DateTime estimatedDeliveryDate, DateTime? actualDeliveryDate, Order? order = null)
        {
            return new Shipping
            (
                id,
                orderId,
                order,
                carrierName,
                trackingNumber,
                status,
                estimatedDeliveryDate,
                actualDeliveryDate
            );
        }

        public void MarkAsShipped()
        {
            if (Status != ShippingStatus.Preparing) throw new DomainException("Only preparing shipments can be shipped.");
            Status = ShippingStatus.Shipped;
        }

        public void MarkAsDelivered()
        {
            if (Status != ShippingStatus.Shipped) throw new DomainException("Only shipped orders can be delivered.");
            Status = ShippingStatus.Delivered;
            ActualDeliveryDate = DateTime.UtcNow;
        }
    }
}
