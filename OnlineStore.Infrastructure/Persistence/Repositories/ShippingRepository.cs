using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Shipping.Queries;
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

        public async Task<PagedResult<Shipping>> GetAllAsync(GetShippingsQuery query)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("dbo.usp_GetShippings", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ShippingStatus", SqlDbType.SmallInt).Value = (object?)(int?)query.ShippingStatus ?? DBNull.Value;

            command.Parameters.Add("@CarrierName", SqlDbType.NVarChar, 100).Value = (object?)query.CarrierName ?? DBNull.Value;

            command.Parameters.Add("@TrackingNumber", SqlDbType.NVarChar, 50).Value = (object?)query.TrackingNumber ?? DBNull.Value;

            command.Parameters.Add("@OrderId", SqlDbType.Int).Value = (object?)query.OrderId ?? DBNull.Value;

            command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = query.PageNumber;

            command.Parameters.Add("@PageSize", SqlDbType.Int).Value = query.PageSize;

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<Shipping> shippings = [];

            int totalCount = 0;

            while (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));

                shippings.Add(MapShipping(reader));
            }

            return new PagedResult<Shipping>
            {
                Items = shippings,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Page = query.PageNumber
            };
        }

        public async Task<Shipping?> GetByIdAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetShippingByID", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ShippingId", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapShipping(reader);
            }

            return null;
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