using Microsoft.Data.SqlClient;
using System.Data;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Product.Queries;

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

        public async Task<PagedResult<Product>> GetProductsAsync(GetProductsQuery query)
        {
            List<Product> products = [];
            int totalCount = 0;

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetProducts", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Page", query.Page);
            command.Parameters.AddWithValue("@PageSize", query.PageSize);

            command.Parameters.AddWithValue("@Search", (object?)query.Search ?? DBNull.Value);

            command.Parameters.AddWithValue("@CategoryId", (object?)query.CategoryId ?? DBNull.Value);

            SqlParameter minPrice = command.Parameters.Add("@MinPrice", SqlDbType.Decimal);
            minPrice.Precision = 18;
            minPrice.Scale = 2;
            minPrice.Value = (object?)query.MinPrice ?? DBNull.Value;

            SqlParameter maxPrice = command.Parameters.Add("@MaxPrice", SqlDbType.Decimal);
            maxPrice.Precision = 18;
            maxPrice.Scale = 2;
            maxPrice.Value = (object?)query.MaxPrice ?? DBNull.Value;

            command.Parameters.AddWithValue("@SortBy", (object?)query.SortBy ?? "Newest");

            command.Parameters.AddWithValue("@Descending", query.Descending);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Category category = Category.Load
                (
                    (int)reader["CategoryId"],
                    (string)reader["CategoryName"],
                    (string)reader["CategoryDescription"],
                    reader["ParentCategoryId"] == DBNull.Value ? null : Convert.ToInt32(reader["ParentCategoryId"]),
                    Convert.ToInt32(reader["DisplayOrder"]),
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

                products.Add(product);

                if (totalCount == 0)
                    totalCount = (int)reader["TotalCount"];
            }

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
