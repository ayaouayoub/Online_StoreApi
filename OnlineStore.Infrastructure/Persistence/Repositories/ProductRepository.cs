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

        public Task<int> GetByIdAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
