using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Authorization
{
    public static class Policies
    {
        public const string Authenticated = "Authenticated";
        public const string ShippingView = "ShippingView";
        public const string CustomerView = "CustomerView";
    }
}
