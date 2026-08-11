using Microsoft.Data.SqlClient;
using System.Data;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PaymentMethodRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PaymentMethod?> GetByIdAsync(int id)
        {
            PaymentMethod? paymentMethod = null;

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetPaymentMethodByID", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PaymentMethodId", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                paymentMethod = PaymentMethod.Load
                (
                    (int)reader["PaymentMethodId"],
                    (string)reader["Name"],
                    reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                    (bool)reader["IsActive"],
                    (DateTime)reader["CreatedAt"]
                );
            }

            return paymentMethod;
        }
    }
}
