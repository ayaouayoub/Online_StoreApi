using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Dtos;
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

            if (await reader.ReadAsync())
            {
                user = User.Load
                (
                    id: (int)reader["UserId"],
                    name: (string)reader["Name"],
                    username: (string)reader["Username"],
                    passwordHash: (string)reader["PasswordHash"],
                    roleId: (int)reader["RoleId"],
                    isActive: (bool)reader["IsActive"],
                    createdAt: (DateTime)reader["CreatedAt"]
                );
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
                    name: (string)reader["Name"]
                )
            );
        }
    }
}
