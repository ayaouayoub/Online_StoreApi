using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class Review
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public Product? Product { get; private set; }
        public int CustomerId { get; private set; }
        public Customer? Customer { get; private set; }
        public string? ReviewText { get; private set; }
        public decimal Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public int? DeletedById { get; private set; }
        public User? DeletedByUser { get; private set; }

        public string? DeleteReason { get; private set; }

        private Review(int productId, int customerId, decimal rating, string? reviewText)
        {
            ValidateProductId(productId);
            ValidateCustomerId(customerId);
            ValidateRating(rating);
            ValidateReviewText(reviewText);

            ProductId = productId;
            CustomerId = customerId;
            Rating = rating;
            ReviewText = reviewText?.Trim();

            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public static Review Create(Product product, Customer customer, decimal rating, string? reviewText = null)
        {
            ArgumentNullException.ThrowIfNull(product);
            ArgumentNullException.ThrowIfNull(customer);

            return new Review(product.Id, customer.Id, rating, reviewText)
            {
                Product = product,
                Customer = customer
            };
        }

        private Review(int id, int productId, Product? product, int customerId, Customer? customer, string? reviewText, decimal rating, DateTime createdAt, DateTime? updatedAt, bool isDeleted, DateTime? deletedAt, int? deletedById, User? deletedByUser, string? deleteReason)
        {
            ValidateProductId(productId);
            ValidateCustomerId(customerId);
            ValidateRating(rating);
            ValidateReviewText(reviewText);
            ValidateDeleteReason(deleteReason);

            if (id <= 0) throw new DomainException("ReviewId must be greater than zero.");

            if (isDeleted && deletedAt is null) throw new DomainException("Deleted review must have DeletedAt.");

            if (isDeleted && deletedById is null) throw new DomainException("Deleted review must have DeletedBy.");

            Id = id;
            ProductId = productId;
            Product = product;
            CustomerId = customerId;
            Customer = customer;
            ReviewText = reviewText;
            Rating = rating;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            DeletedAt = deletedAt;
            DeletedById = deletedById;
            DeletedByUser = deletedByUser;
            DeleteReason = deleteReason;
        }

        public static Review Load(int id, int productId, Product? product, int customerId,Customer? customer, string? reviewText, decimal rating, DateTime createdAt, DateTime? updatedAt, bool isDeleted, DateTime? deletedAt, int? deletedById, User? deletedByUser, string? deleteReason)
        {
            return new Review
            (
                id,
                productId,
                product,
                customerId,
                customer,
                reviewText,
                rating,
                createdAt,
                updatedAt,
                isDeleted,
                deletedAt,
                deletedById,
                deletedByUser,
                deleteReason
            );
        }

        public void Update(decimal rating, string? reviewText)
        {
            EnsureNotDeleted();

            ValidateRating(rating);
            ValidateReviewText(reviewText);

            Rating = rating;
            ReviewText = reviewText?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete(User deletedBy, string? deleteReason)
        {
            if (deletedBy.RoleId == (int)RoleType.SuperAdmin)
                if (string.IsNullOrWhiteSpace(deleteReason)) 
                    throw new DomainException("Delete reason is required when an administrator deletes a review.");

            EnsureNotDeleted();
            ArgumentNullException.ThrowIfNull(deletedBy);
            ValidateDeleteReason(deleteReason);
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedByUser = deletedBy;
            DeletedById = deletedBy.Id;
            DeleteReason = deleteReason?.Trim();
        }

        public void Restore()
        {
            if (!IsDeleted) throw new DomainException("Review is not deleted.");
            IsDeleted = false;
            DeletedAt = null;
            DeletedById = null;
            DeletedByUser = null;
            DeleteReason = null;
            UpdatedAt = DateTime.UtcNow;
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted) throw new DomainException("Cannot modify a deleted review.");
        }

        private static void ValidateProductId(int productId)
        {
            if (productId <= 0) throw new DomainException("ProductId must be greater than zero.");
        }

        private static void ValidateCustomerId(int customerId)
        {
            if (customerId <= 0) throw new DomainException("CustomerId must be greater than zero.");
        }

        private static void ValidateRating(decimal rating)
        {
            if (rating < 1 || rating > 5) throw new DomainException("Rating must be between 1 and 5.");
        }

        private static void ValidateReviewText(string? reviewText)
        {
            if (reviewText is not null && reviewText.Length > 500) throw new DomainException("Review text cannot exceed 500 characters.");
        }

        private static void ValidateDeleteReason(string? deleteReason)
        {
            if (deleteReason is not null && deleteReason.Length > 200) throw new DomainException("Delete reason cannot exceed 200 characters.");
        }
    }
}