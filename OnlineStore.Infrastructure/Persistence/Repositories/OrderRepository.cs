using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Order order)
        {
            await using var connection = _connectionFactory.CreateConnection();

            await using var command = new SqlCommand("dbo.usp_CreateOrder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = order.CustomerId;

            var totalAmountParameter = command.Parameters.Add("@TotalAmount", SqlDbType.Decimal);

            totalAmountParameter.Precision = 18;
            totalAmountParameter.Scale = 2;
            totalAmountParameter.Value = order.TotalAmount;

            command.Parameters.Add("@Status", SqlDbType.TinyInt).Value = (byte)order.Status;

            var itemsTable = CreateItemsTable(order);

            var itemsParameter = command.Parameters.Add("@Items",SqlDbType.Structured);

            itemsParameter.TypeName = "dbo.OrderItemType";
            itemsParameter.Value = itemsTable;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            if (result is null || result == DBNull.Value)throw new InvalidOperationException("Failed to create order.");

            return Convert.ToInt32(result);
        }

        private static DataTable CreateItemsTable(Order order)
        {
            var table = new DataTable();

            table.Columns.Add("ProductId", typeof(int));

            table.Columns.Add("Quantity", typeof(int));

            table.Columns.Add("UnitPrice", typeof(decimal));

            table.Columns.Add("TotalPrice", typeof(decimal));

            foreach (var item in order.Items)
            {
                table.Rows.Add
                (
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice
                );
            }

            return table;
        }
    }
}
