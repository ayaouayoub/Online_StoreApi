using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class Category
    {
        public int Id { get; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public int? ParentId { get; private set; }
        public Category? Parent { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; }

        private Category(int id, string name, string? description, int? parentId, Category? parent, int displayOrder, bool isActive, DateTime createdAt)
        {
            _ValidateName(name);
            if (parent is not null) _ValidateParent(parent);
            _ValidateOrder(displayOrder);
            Id = id;
            Name = name;
            Description = description;
            ParentId = parentId;
            Parent = parent;
            DisplayOrder = displayOrder;
            IsActive = isActive;
            CreatedAt = createdAt;
        }

        public static Category Create(string name, int displayOrder, string? description = null, Category? parent = null)
        {
            return new Category(-1, name, description, parent?.Id, parent, displayOrder, true, DateTime.UtcNow);
        }

        public static Category Load(int id, string name, string description, int? parentId, int displayOrder, bool isActive, DateTime createdAt)
        {
            return new Category(id, name, description, parentId, null, displayOrder, isActive, createdAt);
        }

        public void  ChangeName(string name)
        {
            if (name == Name) return;
            _ValidateName(name);
            Name = name;
        }

        public void ChangeDescription(string description)
        {
            if (Description == description) return;
            Description = description;
        }

        public void ChangeParent(Category parent)
        {
            _ValidateParent(parent);
            Parent = parent;
            ParentId = parent.Id;
        }

        public void ChangeDisplayOrder(int order)
        {
            if (order == DisplayOrder) return;
            _ValidateOrder(order);
            DisplayOrder = order;
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("Category is already active.");
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Category is already inactive.");
            IsActive = false;
        }

        private static void _ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");
        }

        private static void _ValidateOrder(int order)
        {
            if (order < 0) throw new DomainException("display order number cannot be negative.");
        }

        private void _ValidateParent(Category parent)
        {
            if (parent.Id == Id) throw new DomainException("The category cannot be the parent of itself.");
        }
    }
}
