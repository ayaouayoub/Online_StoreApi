using Microsoft.Data.SqlClient;
using System.Data;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_CreateProduct", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductName", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = product.Price;
            command.Parameters["@Price"].Precision = 18;
            command.Parameters["@Price"].Scale = 2;
            command.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
            command.Parameters.AddWithValue("@MainImageURL", (object?)product.MainImageUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

            DataTable imagesTable = _CreateImagesTable(product.Images);

            SqlParameter imagesParameter = command.Parameters.AddWithValue("@Images", imagesTable);
            imagesParameter.SqlDbType = SqlDbType.Structured;
            imagesParameter.TypeName = "dbo.Images";

            await connection.OpenAsync();

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        private static DataTable _CreateImagesTable(IEnumerable<ProductImage> images)
        {
            DataTable table = new();

            table.Columns.Add("ImageURL", typeof(string));
            table.Columns.Add("ImageOrder", typeof(int));

            foreach (ProductImage image in images)
            {
                table.Rows.Add(image.Url, image.ImageOrder);
            }

            return table;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetProductByID", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ProductId", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            Category category = Category.Load
            (
                (int)reader["CategoryId"],
                (string)reader["CategoryName"],
                reader["CategoryDescription"] == DBNull.Value ? null : (string)reader["CategoryDescription"],
                reader["ParentCategoryId"] == DBNull.Value ? null : (int?)reader["ParentCategoryId"],
                (int)reader["DisplayOrder"],
                (bool)reader["IsActive"],
                (DateTime)reader["CreatedAt"]
            );

            Product product = Product.Load
            (
                (int)reader["ProductId"],
                (string)reader["ProductName"],
                reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                (decimal)reader["Price"],
                (int)reader["QuantityInStock"],
                reader["ImageURL"] == DBNull.Value ? null : (string)reader["ImageURL"],
                category.Id,
                category
            );

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                product.AddImage(ProductImage.Load
                (
                    (int)reader["ProductImageId"],
                    (string)reader["ImageURL"],
                    (short)reader["ImageOrder"]
                ));
            }

            return product;
        }
    }
}
