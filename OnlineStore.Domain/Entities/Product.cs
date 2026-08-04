using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class Product
    {
        private readonly List<ProductImage> _images = [];
        public int Id { get; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int QuantityInStock { get; private set; }
        public string? MainImageUrl { get; private set; }
        public IReadOnlyCollection<ProductImage> Images => _images;
        public int CategoryId {  get; private set; }
        public Category? Category {  get; private set; }

        private Product(int id, string name, string? description, decimal price, int quantityInStock, string? mainImageUrl, int categoryId, Category? category)
        {
            _ValidateName(name);
            _ValidatePrice(price);
            _ValidateQuantityInStock(quantityInStock);
            if (category is not null && category.Id != categoryId) throw new DomainException("Category id mismatch.");
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            QuantityInStock = quantityInStock;
            MainImageUrl = mainImageUrl;
            CategoryId = categoryId;
            Category = category;
        }

        public static Product Create(string name, decimal price, int quantityInStock, Category category, string? description = null, string? mainImageUrl = null)
        {
            ArgumentNullException.ThrowIfNull(category);
            return new Product(-1, name, description, price, quantityInStock, mainImageUrl, category.Id, category);
        }

        public static Product Load(int id, string name, string? description, decimal price, int quantityInStock, string? mainImageUrl, int categoryId, Category? category)
        {
            return new Product(id, name, description, price, quantityInStock, mainImageUrl, categoryId, category);
        }

        public void ChangeName(string name)
        {
            if (name == Name) return;
            _ValidateName(name);
            Name = name;
        }

        public void ChangeDescription(string? description)
        {
            if (Description == description) return;
            Description = description;
        }

        public void ChangePrice(decimal price)
        {
            if (Price == price) return;
            _ValidatePrice(price);
            Price = price;
        }

        public void ChangeQuantityInStock(int quantityInStock)
        {
            if (QuantityInStock == quantityInStock) return;
            _ValidateQuantityInStock(quantityInStock);
            QuantityInStock = quantityInStock;
        }

        public void ChangeCategory(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            Category = category;
            CategoryId = category.Id;
        }

        public void ChangeMainImage(string? url)
        {
            if (MainImageUrl == url) return;
            MainImageUrl = url;
        }

        public void AddImage(ProductImage image)
        {
            ArgumentNullException.ThrowIfNull(image);
            if (_images.Any(i => i.Url == image.Url)) throw new DomainException("Image already exists.");
            if (_images.Any(i => i.ImageOrder == image.ImageOrder)) throw new DomainException("Image order already exists.");
            _images.Add(image);
        }

        public void RemoveImageById(int imageId)
        {
            ProductImage? image = _images.FirstOrDefault(i => i.Id == imageId);

            if (image is null) throw new DomainException("Image not found.");

            _images.Remove(image);
        }

        private static void _ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");
        }

        private static void _ValidatePrice(decimal price)
        {
            if (price < 0) throw new DomainException("price cannot be negative");
        }

        private static void _ValidateQuantityInStock(int quantityInStock)
        {
            if (quantityInStock < 0) throw new DomainException("quantity in stock cannot be negative");
        }
    }
}
