using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class ShippingRepository : IShippingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ShippingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Shipping?> GetByOrderIdAsync(int orderId)
        {
            await using var connection = _connectionFactory.CreateConnection();

            await using var command = new SqlCommand("usp_GetShippingByOrderId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return MapShipping(reader);
        }

        public async Task<Shipping?> CreateAsync(Shipping shipping)
        {
            await using var connection = _connectionFactory.CreateConnection();

            await using var command = new SqlCommand("usp_CreateShipping", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@OrderId", SqlDbType.Int).Value = shipping.OrderId;

            command.Parameters.Add("@CarrierName", SqlDbType.NVarChar, 100).Value = shipping.CarrierName;

            command.Parameters.Add("@TrackingNumber", SqlDbType.NVarChar, 50).Value = shipping.TrackingNumber;

            command.Parameters.Add("@EstimatedDeliveryDate", SqlDbType.DateTime2).Value = shipping.EstimatedDeliveryDate;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return MapShipping(reader);
        }

        private static Shipping MapShipping(SqlDataReader reader)
        {
            int shippingId = reader.GetOrdinal("ShippingId");

            int orderId = reader.GetOrdinal("OrderId");

            int carrierName = reader.GetOrdinal("CarrierName");

            int trackingNumber = reader.GetOrdinal("TrackingNumber");

            int status = reader.GetOrdinal("ShippingStatus");

            int estimatedDate = reader.GetOrdinal("EstimatedDeliveryDate");

            int actualDate = reader.GetOrdinal("ActualDeliveryDate");

            return Shipping.Load
            (
                id: reader.GetInt32(shippingId),
                orderId: reader.GetInt32(orderId),
                carrierName: reader.GetString(carrierName),
                trackingNumber: reader.GetString(trackingNumber),
                status: (ShippingStatus)reader.GetInt16(status),
                estimatedDeliveryDate: reader.GetDateTime(estimatedDate),
                actualDeliveryDate: reader.IsDBNull(actualDate) ? null : reader.GetDateTime(actualDate)
            );
        }
    }
}