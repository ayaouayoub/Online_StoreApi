using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineStore.Application.Interfaces.Data;

namespace OnlineStore.Infrastructure.Persistence.Connection
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
