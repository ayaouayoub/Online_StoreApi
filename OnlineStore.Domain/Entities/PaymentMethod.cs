using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class PaymentMethod
    {
        public int Id { get; }
        public string Name { get; } = null!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; }

        private PaymentMethod(int id, string name, string? description, bool isActive, DateTime? createdAt = null)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new DomainException("Payment method name cannot be null or white space."); }
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            CreatedAt = createdAt ?? DateTime.UtcNow;
        }
        public static PaymentMethod Create(string name, string? description = null)
        {
            return new PaymentMethod(-1, name, description, true);
        }

        public static PaymentMethod Load(int id, string name, string? description, bool isActive, DateTime createdAt)
        {
            return new PaymentMethod(id, name, description, isActive, createdAt);
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }

        public void ChangeDescription(string description)
        {
            if (Description == description) return;
            Description = description;
        }
    }
}
