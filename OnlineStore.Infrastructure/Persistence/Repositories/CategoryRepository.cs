using Microsoft.Data.SqlClient;
using System.Data;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            Category? category = null;

            using SqlConnection connection = _connectionFactory.CreateConnection();
            using SqlCommand command = new("usp_GetCategoryByID", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CategoryId", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                category = Category.Load
                (
                    id: (int)reader["CategoryId"],
                    name: (string)reader["CategoryName"],
                    description: (string)reader["Description"],
                    parentId: reader["ParentCategoryId"] == DBNull.Value ? null : (int)reader["ParentCategoryId"],
                    displayOrder: (int)reader["DisplayOrder"],
                    isActive: (bool)reader["IsActive"],
                    createdAt: (DateTime)reader["CreatedAt"]
                );
            }

            return category;
        }
    }
}
