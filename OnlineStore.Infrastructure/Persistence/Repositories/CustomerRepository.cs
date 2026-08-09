using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();

            await using var command =new SqlCommand("dbo.usp_GetCustomerById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) return null;

            return MapCustomer(reader);
        }

        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            await using var connection = _connectionFactory.CreateConnection();

            await using var command = new SqlCommand("dbo.usp_GetCustomerByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) return null;

            return MapCustomer(reader);
        }

        private static Customer MapCustomer(SqlDataReader reader)
        {
            return Customer.Load
            (
                Id: reader.GetInt32(reader.GetOrdinal("CustomerId")),
                email: reader.GetString(reader.GetOrdinal("Email")),
                address: reader.GetString(reader.GetOrdinal("Address")),
                UserId: reader.GetInt32(reader.GetOrdinal("UserId")),
                phone: reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString( reader.GetOrdinal("Phone"))
            );
        }
    }
}
