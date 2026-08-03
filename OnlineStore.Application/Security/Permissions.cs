using System.Reflection;

namespace OnlineStore.Application.Security
{
    public static class Permissions
    {
        public static class Users
        {
            public const string View = "Users.View";
            public const string Create = "Users.Create";
            public const string Update = "Users.Update";
            public const string Delete = "Users.Delete";
        }

        public static class Roles
        {
            public const string View = "Roles.View";
            public const string Create = "Roles.Create";
            public const string Update = "Roles.Update";
            public const string Delete = "Roles.Delete";
        }

        public static class Customers
        {
            public const string View = "Customers.View";
            public const string Create = "Customers.Create";
            public const string Update = "Customers.Update";
            public const string Delete = "Customers.Delete";
        }

        public static class Categories
        {
            public const string View = "Categories.View";
            public const string Create = "Categories.Create";
            public const string Update = "Categories.Update";
            public const string Delete = "Categories.Delete";
        }

        public static class Products
        {
            public const string View = "Products.View";
            public const string Create = "Products.Create";
            public const string Update = "Products.Update";
            public const string Delete = "Products.Delete";
        }

        public static class Reviews
        {
            public const string View = "Reviews.View";
            public const string Delete = "Reviews.Delete";
        }

        public static class Orders
        {
            public const string View = "Orders.View";
            public const string Create = "Orders.Create";
            public const string Update = "Orders.Update";
            public const string Delete = "Orders.Delete";
        }

        public static class OrderItems
        {
            public const string View = "OrderItems.View";
            public const string Create = "OrderItems.Create";
            public const string Update = "OrderItems.Update";
            public const string Delete = "OrderItems.Delete";
        }

        public static class Payments
        {
            public const string View = "Payments.View";
            public const string Create = "Payments.Create";
            public const string Update = "Payments.Update";
            public const string Delete = "Payments.Delete";
        }

        public static class Shipping
        {
            public const string View = "Shipping.View";
            public const string Create = "Shipping.Create";
            public const string Update = "Shipping.Update";
            public const string Delete = "Shipping.Delete";
        }

        public static IEnumerable<string> GetAll()
        {
            return typeof(Permissions).GetNestedTypes(BindingFlags.Public)
                        .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .Where(field => field.IsLiteral && !field.IsInitOnly)
                        .Select(field => field.GetRawConstantValue()!.ToString()!));
        }
    }
}