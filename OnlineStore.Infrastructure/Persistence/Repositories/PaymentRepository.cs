using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Payment.Queries;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.ValueObjs;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PaymentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResult<Payment>> GetPagedAsync(GetPaymentsQuery query)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("usp_GetPaymentsPaged", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@OrderId", SqlDbType.Int).Value = (object?)query.OrderId ?? DBNull.Value;

            command.Parameters.Add("@PaymentMethodId", SqlDbType.Int).Value = (object?)query.PaymentMethodId ?? DBNull.Value;

            command.Parameters.Add("@Status", SqlDbType.TinyInt).Value = query.Status.HasValue ? (byte)query.Status.Value : DBNull.Value;

            command.Parameters.Add("@Currency", SqlDbType.NVarChar).Value = (object?)query.Currency ?? DBNull.Value;

            command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = query.PageNumber;

            command.Parameters.Add("@PageSize", SqlDbType.Int).Value = query.PageSize;

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) throw new Exception("Failed to read total count.");

            int totalCount = reader.GetInt32(0);

            await reader.NextResultAsync();

            List<Payment> payments = [];

            while (await reader.ReadAsync())
            {
                payments.Add(MapPayment(reader));
            }

            return new PagedResult<Payment>
            {
                Items = payments,
                TotalCount = totalCount,
                Page = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("dbo.usp_GetPaymentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@PaymentId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) return null;

            return MapPayment(reader);
        }

        private static Payment MapPayment(SqlDataReader reader)
        {
            var paymentMethod = PaymentMethod.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("PM_PaymentMethodId")),
                name: reader.GetString(reader.GetOrdinal("PM_Name")),
                description: reader.IsDBNull(reader.GetOrdinal("PM_Description")) ? null : reader.GetString(reader.GetOrdinal("PM_Description")),
                isActive: reader.GetBoolean(reader.GetOrdinal("PM_IsActive")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("PM_CreatedAt"))
            );

            var money = new Money
            (
                reader.GetDecimal(reader.GetOrdinal("Amount")),
                new Currency(reader.GetString(reader.GetOrdinal("Currency")))
            );

            return Payment.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("PaymentId")),
                orderId: reader.GetInt32(reader.GetOrdinal("OrderId")),
                money: money,
                paymentMethodId: reader.GetInt32(reader.GetOrdinal("PaymentMethodId")),
                status: (PaymentStatus)reader.GetByte(reader.GetOrdinal("Status")),
                transactionDate: reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                transactionId: reader.IsDBNull(reader.GetOrdinal("TransactionId")) ? null : reader.GetString( reader.GetOrdinal("TransactionId")),
                paymentMethod: paymentMethod
            );
        }
    }
}