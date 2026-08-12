using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
