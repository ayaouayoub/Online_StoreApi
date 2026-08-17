using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RoleRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Role> CreateAsync(Role role)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_CreateRole", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@RoleName", role.Name);

            await connection.OpenAsync();

            int roleId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return Role.Load(roleId, role.Name);
        }

        public async Task<IReadOnlyCollection<Role>> GetAllAsync()
        {
            List<Role> roles = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetRoles", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add
                (
                    Role.Load
                    (
                        id: reader.GetInt32(reader.GetOrdinal("RoleId")),
                        name: reader.GetString(reader.GetOrdinal("RoleName"))
                    )
                );
            }

            return roles.AsReadOnly();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetRoleById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RoleId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) return null;

            return Role.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("RoleId")),
                name: reader.GetString(reader.GetOrdinal("RoleName"))
            );
        }
    }
}
