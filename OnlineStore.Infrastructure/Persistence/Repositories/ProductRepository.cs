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

        public async Task<Product> CreateProductAsync(Product product)
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

            DataTable imagesTable = CreateImagesTable(product.Images);

            SqlParameter imagesParameter = command.Parameters.AddWithValue("@Images", imagesTable);
            imagesParameter.SqlDbType = SqlDbType.Structured;
            imagesParameter.TypeName = "dbo.Images";

            await connection.OpenAsync();

            int productId = Convert.ToInt32(await command.ExecuteScalarAsync());

            var createdProducte = Product.Load(productId, product.Name, product.Description, product.Price, product.QuantityInStock, product.MainImageUrl, product.CategoryId, product.Category);
            createdProducte.SetImages(product.Images);

            return createdProducte;
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
                Product product = MapProduct(reader);

                products.Add(product);

                if (totalCount == 0) totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
            }


            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));

                Product? product = products.FirstOrDefault(p => p.Id == productId);

                if (product is null) continue;

                ProductImage image = ProductImage.Load
                (
                    id: reader.GetInt32(reader.GetOrdinal("ProductImageId")),
                    url: reader.GetString(reader.GetOrdinal("ImageURL")),
                    imageOrder: reader.GetInt16(reader.GetOrdinal("ImageOrder"))
                );

                product.AddImage(image);
            }

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task UpdateStockAsync(int productId, int quantityChange)
        {
            await using SqlConnection connection = _connectionFactory.CreateConnection();

            await using SqlCommand command = new("dbo.usp_UpdateProductStock", connection) { CommandType = CommandType.StoredProcedure };

            command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

            command.Parameters.Add("@QuantityChange", SqlDbType.Int).Value = quantityChange;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_UpdateProduct", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ProductId", SqlDbType.Int).Value = product.Id;


            command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 100).Value = product.Name;


            command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = product.Description;

            SqlParameter priceParameter = command.Parameters.Add("@Price", SqlDbType.Decimal);

            priceParameter.Precision = 18;
            priceParameter.Scale = 2;
            priceParameter.Value = product.Price;

            command.Parameters.Add("@ImageURL", SqlDbType.NVarChar, 200).Value = (object?)product.MainImageUrl ?? DBNull.Value;

            command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = product.CategoryId;

            DataTable imagesTable = CreateImagesTable(product.Images);

            SqlParameter imagesParameter =command.Parameters.Add("@Images", SqlDbType.Structured);

            imagesParameter.TypeName = "dbo.Images";

            imagesParameter.Value = imagesTable;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new Exception("Failed to load product");
            }

            Product result = MapProduct(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                result.AddImage
                (
                    ProductImage.Load
                    (
                        id: reader.GetInt32(reader.GetOrdinal("ProductImageId")),
                        url: reader.GetString(reader.GetOrdinal("ImageURL")),
                        imageOrder: reader.GetInt16(reader.GetOrdinal("ImageOrder"))
                    )
                );
            }

            return result;
        }

        private static Product MapProduct(SqlDataReader reader)
        {
            var category = Category.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("CategoryId")),
                name: reader.GetString(reader.GetOrdinal("CategoryName")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("CategoryCreatedAt")),
                description: reader.GetString(reader.GetOrdinal("CategoryDescription")),
                displayOrder: reader.GetInt32(reader.GetOrdinal("CategoryDisplayOrder")),
                isActive: reader.GetBoolean(reader.GetOrdinal("IsActive")),
                parentId: reader["ParentCategoryId"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("ParentCategoryId"))
            );

            return Product.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("ProductId")),
                name: reader.GetString(reader.GetOrdinal("ProductName")),
                description: reader.GetString(reader.GetOrdinal("Description")),
                price: reader.GetDecimal(reader.GetOrdinal("Price")),
                quantityInStock: reader.GetInt32(reader.GetOrdinal("QuantityInStock")),
                mainImageUrl: reader.GetString(reader.GetOrdinal("ImageURL")),
                categoryId: reader.GetInt32(reader.GetOrdinal("CategoryId")),
                category
            );
        }

        private static DataTable CreateImagesTable(IEnumerable<ProductImage> images)
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
    }
}
