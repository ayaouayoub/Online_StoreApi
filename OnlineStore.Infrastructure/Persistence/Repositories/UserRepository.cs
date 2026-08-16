using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.User.Queries;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using static OnlineStore.Application.Security.Permissions;

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

        private static void AddPermissionIfExists(SqlDataReader reader, UserDto user, int permissionIdOrdinal, int permissionCodeOrdinal, int permissionNameOrdinal)
        {
            if (reader.IsDBNull(permissionIdOrdinal)) return;

            int permissionId = reader.GetInt32(permissionIdOrdinal);

            if (user.Permissions.Any(p => p.Id == permissionId)) return;

            user.Permissions.Add(new PermissionDto
            {
                Id = permissionId,
                Code = reader.GetString(permissionCodeOrdinal),
                Name = reader.GetString(permissionNameOrdinal)
            });
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

        public async Task<PagedResult<UserDto>> GetUsersAsync(GetUsersQuery query)
        {
            Dictionary<int, UserDto> users = [];

            using var connection = _connectionFactory.CreateConnection();

            using var command = new SqlCommand("usp_GetUsers", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = (object?)query.Username ?? DBNull.Value;

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = (object?)query.Name ?? DBNull.Value;

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = (object?)query.IsActive ?? DBNull.Value;

            command.Parameters.Add("@RoleId", SqlDbType.Int).Value = (object?)query.RoleId ?? DBNull.Value;

            command.Parameters.Add("@From", SqlDbType.DateTime2).Value = (object?)query.From ?? DBNull.Value;

            command.Parameters.Add("@To", SqlDbType.DateTime2).Value = (object?)query.To ?? DBNull.Value;

            command.Parameters.Add("@Page", SqlDbType.Int).Value = query.Page;

            command.Parameters.Add("@PageSize", SqlDbType.Int).Value = query.PageSize;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            int totalCount = 0;

            int userIdOrdinal = reader.GetOrdinal("UserId");
            int nameOrdinal = reader.GetOrdinal("Name");
            int usernameOrdinal = reader.GetOrdinal("Username");
            int roleIdOrdinal = reader.GetOrdinal("RoleId");
            int isActiveOrdinal = reader.GetOrdinal("IsActive");
            int createdAtOrdinal = reader.GetOrdinal("CreatedAt");
            int totalCountOrdinal = reader.GetOrdinal("TotalCount");

            int permissionIdOrdinal = reader.GetOrdinal("PermissionId");
            int permissionCodeOrdinal = reader.GetOrdinal("PermissionCode");
            int permissionNameOrdinal = reader.GetOrdinal("PermissionName");

            while (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(totalCountOrdinal);

                int userId = reader.GetInt32(userIdOrdinal);

                if (!users.TryGetValue(userId, out UserDto? user))
                {
                    user = new UserDto
                    {
                        Id = userId,
                        Name = reader.GetString(nameOrdinal),
                        Username = reader.GetString(usernameOrdinal),
                        RoleType = (RoleType)reader.GetInt32(roleIdOrdinal),
                        IsActive = reader.GetBoolean(isActiveOrdinal),
                        CreatedAt = reader.GetDateTime(createdAtOrdinal)
                    };

                    users.Add(userId, user);
                }

                AddPermissionIfExists
                (
                    reader,
                    user,
                    permissionIdOrdinal,
                    permissionCodeOrdinal,
                    permissionNameOrdinal
                );
            }

            return new PagedResult<UserDto>
            {
                Items = [..users.Values],
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        private static void AddPermissionIdsParameter(IReadOnlyCollection<Permission> permissions, SqlCommand command)
        {
            var table = new DataTable();

            table.Columns.Add("PermissionId", typeof(int));

            foreach (var permission in permissions) table.Rows.Add(permission.Id);

            var parameter = command.Parameters.Add("@PermissionIds", SqlDbType.Structured);

            parameter.TypeName = "dbo.PermissionIdTable";
            parameter.Value = table;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_DeactivateUser", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UserId", id);

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_ActivateUser", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UserId", id);

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> ChangeMyPasswordAsync(int userId, string passwordHash)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_ChangeMyPassword", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_UpdateUser", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UserId", user.Id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Username", user.Username);

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }
    }
}
