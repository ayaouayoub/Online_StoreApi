using System.Data;
using Microsoft.Data.SqlClient;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Handlers.Review.Queries;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Repositories
{
    internal class ReviewRepository : IReviewRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ReviewRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResult<Review>> GetAllAsync(GetReviewsQuery query)
        {
            List<Review> reviews = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetReviews", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ReviewId", SqlDbType.Int).Value = (object?)query.ReviewId ?? DBNull.Value;

            command.Parameters.Add("@ProductId", SqlDbType.Int).Value = (object?)query.ProductId ?? DBNull.Value;

            command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = (object?)query.CustomerId ?? DBNull.Value;

            SqlParameter rating = command.Parameters.Add("@Rating", SqlDbType.Decimal);

            rating.Precision = 2;
            rating.Scale = 1;
            rating.Value = (object?)query.Rating ?? DBNull.Value;

            command.Parameters.Add("@ReviewText", SqlDbType.NVarChar, 500).Value = (object?)query.ReviewText ?? DBNull.Value;

            command.Parameters.Add("@CreatedFrom", SqlDbType.DateTime2).Value = (object?)query.CreatedFrom ?? DBNull.Value;

            command.Parameters.Add("@CreatedTo", SqlDbType.DateTime2).Value = (object?)query.CreatedTo ?? DBNull.Value;

            command.Parameters.Add("@UpdatedFrom", SqlDbType.DateTime2).Value = (object?)query.UpdatedFrom ?? DBNull.Value;

            command.Parameters.Add("@UpdatedTo", SqlDbType.DateTime2).Value = (object?)query.UpdatedTo ?? DBNull.Value;

            command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = (object?)query.IsDeleted ?? DBNull.Value;

            command.Parameters.Add("@DeletedFrom", SqlDbType.DateTime2).Value = (object?)query.DeletedFrom ?? DBNull.Value;

            command.Parameters.Add("@DeletedTo", SqlDbType.DateTime2).Value = (object?)query.DeletedTo ?? DBNull.Value;

            command.Parameters.Add("@DeletedBy", SqlDbType.Int).Value = (object?)query.DeletedBy ?? DBNull.Value;

            command.Parameters.Add("@DeleteReason",SqlDbType.NVarChar, 200).Value = (object?)query.DeleteReason ?? DBNull.Value;

            command.Parameters.Add("@Page", SqlDbType.Int).Value = query.Page;

            command.Parameters.Add("@PageSize", SqlDbType.Int).Value = query.PageSize;


            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            int totalCount = 0;

            while (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                reviews.Add(MapReview(reader));
            }

            return new PagedResult<Review>
            {
                Items = reviews,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetReviewById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ReviewId", SqlDbType.Int).Value = reviewId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return MapReview(reader);
        }

        public async Task<IReadOnlyList<Review>> GetByProductIdAsync(int productId)
        {
            List<Review> reviews = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetReviewsByProductId", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync()) reviews.Add(MapReview(reader));

            return reviews;
        }

        public async Task<IReadOnlyList<Review>> GetByCustomerIdAsync(int customerId)
        {
            List<Review> reviews = [];

            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetReviewsByCustomerId", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync()) reviews.Add(MapReview(reader));

            return reviews;
        }

        public async Task<Review> CreateAsync(Review review)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_CreateReview", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ProductId", SqlDbType.Int).Value = review.ProductId;

            command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = review.CustomerId;

            command.Parameters.Add("@ReviewText", SqlDbType.NVarChar, 500).Value = (object?)review.ReviewText ?? DBNull.Value;

            SqlParameter rating = command.Parameters.Add("@Rating", SqlDbType.Decimal);

            rating.Precision = 2;
            rating.Scale = 1;
            rating.Value = review.Rating;

            await connection.OpenAsync();

            int id = Convert.ToInt32(await command.ExecuteScalarAsync());

            return Review.Load(id, review.ProductId, review.Product, review.CustomerId, review.Customer, review.ReviewText, review.Rating, review.CreatedAt, review.UpdatedAt, review.IsDeleted, review.DeletedAt, review.DeletedById, review.DeletedByUser, review.DeleteReason);
        }

        public async Task UpdateAsync(Review review)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_UpdateReview", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ReviewId", SqlDbType.Int).Value = review.Id;

            SqlParameter rating = command.Parameters.Add("@Rating", SqlDbType.Decimal);

            rating.Precision = 2;
            rating.Scale = 1;
            rating.Value = review.Rating;

            command.Parameters.Add("@ReviewText", SqlDbType.NVarChar,500).Value = (object?)review.ReviewText ?? DBNull.Value;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Review review)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_DeleteReview", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ReviewId", SqlDbType.Int).Value = review.Id;

            command.Parameters.Add("@DeletedBy", SqlDbType.Int).Value = review.DeletedById!.Value;

            command.Parameters.Add("@DeleteReason", SqlDbType.NVarChar, 200).Value = (object?)review.DeleteReason ?? DBNull.Value;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task RestoreAsync(Review review)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_RestoreReview", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ReviewId", SqlDbType.Int).Value = review.Id;

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Review?> GetByIdAsyncIncludingDeleted(int reviewId)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();

            using SqlCommand command = new("usp_GetReviewByIdIncludingDeleted", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@ReviewId", SqlDbType.Int).Value = reviewId;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return MapReview(reader);
        }

        private static Review MapReview(SqlDataReader reader)
        {
            Product product = Product.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("ProductId")),
                name: reader.GetString(reader.GetOrdinal("ProductName")),
                description: reader.IsDBNull(reader.GetOrdinal("ProductDescription")) ? null : reader.GetString(reader.GetOrdinal("ProductDescription")),
                price: reader.GetDecimal(reader.GetOrdinal("ProductPrice")),
                quantityInStock: reader.GetInt32(reader.GetOrdinal("QuantityInStock")),
                mainImageUrl: reader.IsDBNull(reader.GetOrdinal("MainImageUrl")) ? null : reader.GetString(reader.GetOrdinal("MainImageUrl")),
                categoryId: reader.GetInt32(reader.GetOrdinal("CategoryId")),
                category: null
            );

            User customerUser = User.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("CustomerUserId_Value")),
                name: reader.GetString(reader.GetOrdinal("CustomerUserName")),
                username: reader.GetString(reader.GetOrdinal("CustomerUsername")),
                passwordHash: reader.GetString(reader.GetOrdinal("CustomerPasswordHash")),
                roleId: reader.GetInt32(reader.GetOrdinal("CustomerRoleId")),
                isActive: reader.GetBoolean(reader.GetOrdinal("CustomerUserIsActive")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("CustomerUserCreatedAt"))
            );

            Customer customer = Customer.Load
            (
                Id: reader.GetInt32(reader.GetOrdinal("CustomerId")),
                email: reader.GetString(reader.GetOrdinal("CustomerEmail")),
                address: reader.GetString(reader.GetOrdinal("CustomerAddress")),
                UserId: reader.GetInt32(reader.GetOrdinal("CustomerUserId")),
                phone: reader.IsDBNull(reader.GetOrdinal("CustomerPhone")) ? null : reader.GetString(reader.GetOrdinal("CustomerPhone")),
                user: customerUser
            );

            User? deletedByUser = null;

            if (!reader.IsDBNull(reader.GetOrdinal("DeletedBy")))
            {
                deletedByUser = User.Load
                (
                    id: reader.GetInt32(reader.GetOrdinal("DeletedByUserId")),
                    name: reader.GetString(reader.GetOrdinal("DeletedByUserName")),
                    username: reader.GetString(reader.GetOrdinal("DeletedByUsername")),
                    passwordHash: reader.GetString(reader.GetOrdinal("DeletedByPasswordHash")),
                    roleId: reader.GetInt32(reader.GetOrdinal("DeletedByRoleId")),
                    isActive: reader.GetBoolean(reader.GetOrdinal("DeletedByIsActive")),
                    createdAt: reader.GetDateTime(reader.GetOrdinal("DeletedByUserCreatedAt"))
                );
            }

            return Review.Load
            (
                id: reader.GetInt32(reader.GetOrdinal("ReviewId")),
                productId: reader.GetInt32(reader.GetOrdinal("ProductId")),
                product: product,
                customerId: reader.GetInt32(reader.GetOrdinal("CustomerId")),
                customer: customer,
                reviewText: reader.IsDBNull(reader.GetOrdinal("ReviewText")) ? null : reader.GetString(reader.GetOrdinal("ReviewText")),
                rating: reader.GetDecimal(reader.GetOrdinal("Rating")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                updatedAt: reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                isDeleted: reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                deletedAt: reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("DeletedAt")),
                deletedById: reader.IsDBNull(reader.GetOrdinal("DeletedBy")) ? null : reader.GetInt32(reader.GetOrdinal("DeletedBy")),
                deletedByUser: deletedByUser,
                deleteReason: reader.IsDBNull(reader.GetOrdinal("DeleteReason")) ? null : reader.GetString(reader.GetOrdinal("DeleteReason"))
            );
        }
    }
}
