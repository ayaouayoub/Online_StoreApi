using Microsoft.Data.SqlClient;
using System.Data;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Application.Dtos;

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

        public async Task<IReadOnlyList<CategorySummaryDto>> GetCategoriesAsync()
        {
            List<CategorySummaryDto> categories = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetCategories", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new CategorySummaryDto
                {
                    Id = (int)reader["CategoryId"],
                    Name = (string)reader["CategoryName"],

                    Parent = reader["ParentId"] == DBNull.Value ? null
                        : new LookupDto
                        {
                            Id = (int)reader["ParentId"],
                            Name = (string)reader["ParentName"]
                        }
                });
            }

            return categories;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("usp_CreateCategory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.Name;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = (object?)category.Description ?? DBNull.Value;

            command.Parameters.Add("@ParentCategoryId", SqlDbType.Int).Value = (object?)category.ParentId ?? DBNull.Value;

            command.Parameters.Add("@DisplayOrder", SqlDbType.Int).Value = category.DisplayOrder;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Category.Load(Convert.ToInt32(result), category.Name, category.Description, category.ParentId, category.DisplayOrder, category.IsActive, category.CreatedAt);
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("dbo.usp_UpdateCategory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = category.Id;

            command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.Name;

            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = (object?)category.Description ?? DBNull.Value;

            command.Parameters.Add("@ParentCategoryId", SqlDbType.Int).Value = (object?)category.ParentId ?? DBNull.Value;

            command.Parameters.Add("@DisplayOrder", SqlDbType.Int).Value = category.DisplayOrder;

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("dbo.usp_ActivateCategory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("dbo.usp_DeactivateCategory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }

        public async Task<bool> HasActiveChildrenAsync(int categoryId)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("usp_DoesCategoryHaveActiveChildren", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;

            SqlParameter returnParameter = command.Parameters.Add("@ReturnValue", SqlDbType.Int);

            returnParameter.Direction = ParameterDirection.ReturnValue;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return Convert.ToInt32(returnParameter.Value) == 1;
        }
    }
}