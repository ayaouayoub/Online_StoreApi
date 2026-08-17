using Microsoft.Data.SqlClient;
using System.Data;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using static OnlineStore.Application.Security.Permissions;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PaymentMethodRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_CreatePaymentMethod", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", paymentMethod.Name);
            command.Parameters.AddWithValue("@Description", paymentMethod.Description);

            await connection.OpenAsync();

            int paymentMethodId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return PaymentMethod.Load(paymentMethodId, paymentMethod.Name, paymentMethod.Description, paymentMethod.IsActive, paymentMethod.CreatedAt);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_DeactivatePaymentMethod", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PaymentMethodId", id);

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<IReadOnlyCollection<PaymentMethod>> GetAllAsync()
        {
            List<PaymentMethod> paymentMethods = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetPaymentMethods", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                paymentMethods.Add
                (
                    PaymentMethod.Load
                    (
                        id: reader.GetInt32(reader.GetOrdinal("PaymentMethodId")),
                        name: reader.GetString(reader.GetOrdinal("Name")),
                        description: reader.GetString(reader.GetOrdinal("Description")),
                        isActive: reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        createdAt: reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    )
                );
            }

            return paymentMethods.AsReadOnly();
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

        public async Task<bool> UpdateAsync(PaymentMethod paymentMethod)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("usp_UpdatePaymentMethod", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@PaymentMethodId", SqlDbType.Int).Value = paymentMethod.Id;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = (object?)paymentMethod.Description ?? DBNull.Value;

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_ActivatePaymentMethod", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@PaymentMethodId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }
    }
}
