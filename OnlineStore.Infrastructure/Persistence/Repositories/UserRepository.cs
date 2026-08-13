using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            User? user = null;

            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new("usp_GetUserByID", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (user is null)
                {
                    Role role = Role.Load
                    (
                        id: (int)reader["RoleId"],
                        name: (string)reader["RoleName"]
                    );

                    user = User.LoadWithRole
                    (
                        id: (int)reader["UserId"],
                        name: (string)reader["Name"],
                        username: (string)reader["Username"],
                        passwordHash: (string)reader["PasswordHash"],
                        role: role,
                        isActive: (bool)reader["IsActive"],
                        createdAt: (DateTime)reader["CreatedAt"]
                    );
                }

                AddPermissionIfExists(reader, user);
            }

            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            User? user = null;

            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new("usp_GetUserByUserName", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Username", username);

            string s = BCrypt.Net.BCrypt.HashPassword("Admin123");

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (user is null)
                {
                    Role role = Role.Load
                    (
                        id: (int)reader["RoleId"],
                        name: (string)reader["RoleName"]
                    );

                    user = User.LoadWithRole
                    (
                        id: (int)reader["UserId"],
                        name: (string)reader["Name"],
                        username: (string)reader["Username"],
                        passwordHash: (string)reader["PasswordHash"],
                        role: role,
                        isActive: (bool)reader["IsActive"],
                        createdAt: (DateTime)reader["CreatedAt"]
                    );
                }

                AddPermissionIfExists(reader, user);
            }

            return user;
        }

        private static void AddPermissionIfExists(SqlDataReader reader, User? user)
        {
            if (user is null)
                return;

            if (reader["PermissionId"] == DBNull.Value)
                return;

            user.AddPermission
            (
                Permission.Load
                (
                    id: (int)reader["PermissionId"],
                    code: (string)reader["Code"],
                    name: (string)reader["PermissionName"]
                )
            );
        }

        public async Task<int> CreateUserAsync(User user)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("usp_CreateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = user.Name;

            command.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;

            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = user.PasswordHash;

            command.Parameters.Add("@RoleId", SqlDbType.Int).Value = user.RoleId;

            AddPermissionIdsParameter(user.Permissions, command);

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) throw new InvalidOperationException("Failed to create user.");

            return reader.GetInt32(reader.GetOrdinal("UserId"));
        }

        private static void AddPermissionIdsParameter(IReadOnlyCollection<Permission> permissions, SqlCommand command)
        {
            var table = new DataTable();

            table.Columns.Add("PermissionId", typeof(int));

            foreach (var permission in permissions.GroupBy(p => p.Id).Select(g => g.First())) table.Rows.Add(permission.Id);

            var parameter = command.Parameters.Add("@PermissionIds", SqlDbType.Structured);

            parameter.TypeName = "dbo.PermissionIdTable";
            parameter.Value = table;
        }
    }
}
