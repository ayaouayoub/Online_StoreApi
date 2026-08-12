using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Handlers.Customer.Models;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CustomerDetails?> GetByIdAsync(int id)
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

            return MapCustomerDeatils(reader);
        }

        public async Task<CustomerDetails?> GetByUserIdAsync(int userId)
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

            return MapCustomerDeatils(reader);
        }

        public async Task<CustomerDetails> RegisterAsync(User user, Customer customer)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("dbo.usp_RegisterCustomer", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = user.Name;

            command.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;

            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = user.PasswordHash;

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = customer.Email.ToString();

            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)customer.Phone ?? DBNull.Value;

            command.Parameters.Add("@Address", SqlDbType.NVarChar, 200).Value = customer.Address;

            command.Parameters.Add("@CustomerRoleId", SqlDbType.Int).Value = user.RoleId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) throw new DomainException("Failed to register customer.");

            return MapCustomerDeatils(reader);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetCustomerByEmail", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

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
                phone: reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone"))
            );
        }

        private static CustomerDetails MapCustomerDeatils(SqlDataReader reader)
        {
            return new CustomerDetails
            {
                Customer = MapCustomer(reader),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Name")),
                Name = reader.GetString(reader.GetOrdinal("Username")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
