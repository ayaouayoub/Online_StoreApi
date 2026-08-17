using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using static OnlineStore.Application.Security.Permissions;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    internal class PermissionRepository : IPermissionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PermissionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyCollection<Permission>> GetByIdsAsync(IReadOnlyCollection<int> permissionIds)
        {
            if (permissionIds.Count == 0) return [];

            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("usp_GetPermissionsByIds", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            CreatePermissionsIdsTable(permissionIds, command);

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<Permission> permissions = [];

            while (await reader.ReadAsync())
            {
                permissions.Add
                (
                    Permission.Load
                    (
                        reader.GetInt32(reader.GetOrdinal("PermissionId")),
                        reader.GetString(reader.GetOrdinal("Code")),
                        reader.GetString(reader.GetOrdinal("Name"))
                    )
                );
            }

            return permissions;
        }

        private static void CreatePermissionsIdsTable(IReadOnlyCollection<int> permissionIds, SqlCommand command)
        {
            var table = new DataTable();

            table.Columns.Add("PermissionId", typeof(int));

            foreach (int permissionId in permissionIds.Distinct()) table.Rows.Add(permissionId);

            var parameter = command.Parameters.Add("@PermissionIds", SqlDbType.Structured);

            parameter.TypeName = "dbo.PermissionIdTable";
            parameter.Value = table;
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetPermissionById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PermissioId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) return null;

            return Permission.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("PermissionId")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                code: reader.GetString(reader.GetOrdinal("Code"))
            );
        }

        public async Task<IReadOnlyCollection<Permission>> GetAllAsync()
        {
            List<Permission> permissions = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetPermissions", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                permissions.Add
                (
                    Permission.Load
                    (
                        id: reader.GetInt32(reader.GetOrdinal("PermissionId")),
                        name: reader.GetString(reader.GetOrdinal("Name")),
                        code: reader.GetString(reader.GetOrdinal("code"))
                    )
                );
            }

            return permissions.AsReadOnly();
        }

        public async Task<Permission> CreateAsync(Permission permission)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_CreatePermission", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PermissionName", permission.Name);
            command.Parameters.AddWithValue("@PermissionCode", permission.Code);

            await connection.OpenAsync();

            int permissionId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return Permission.Load(permissionId, permission.Code, permission.Name);
        }
    }
}
