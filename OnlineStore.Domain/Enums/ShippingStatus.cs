using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Domain.Enums
{
    public enum ShippingStatus : short
    {
        Preparing = 0,
        Shipped = 1,
        Delivered = 2
    }
}
