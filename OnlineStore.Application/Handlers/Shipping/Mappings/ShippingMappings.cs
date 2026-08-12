using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.Shipping.Mappings
{
    public static class ShippingMappings
    {
        public static ShippingDto ToDto(this Domain.Entities.Shipping shipping)
        {
            return new ShippingDto
            (
                shipping.Id,
                shipping.OrderId,
                shipping.CarrierName,
                shipping.TrackingNumber,
                shipping.Status,
                shipping.EstimatedDeliveryDate,
                shipping.ActualDeliveryDate
            );
        }
    }
}
