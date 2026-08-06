using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public sealed class User
    {
        private readonly HashSet<Permission> _permissions = [];

        public User(int id, string name, string username, string passwordHash, int roleId, Role? role, bool isActive, DateTime createdAt)
        {
            _ValidateUsername(username);
            _ValidatePasswordHash(passwordHash);

            if (role is not null && role.Id != roleId)
                throw new DomainException("Role id mismatch.");

            Id = id;
            Name = name;
            Username = username;
            PasswordHash = passwordHash;
            RoleId = roleId;
            Role = role;
            IsActive = isActive;
            CreatedAt = createdAt;
        }

        public int Id { get; }

        public string Name { get; private set; }

        public string Username { get; private set; }

        public string PasswordHash { get; private set; }

        public int RoleId { get; private set; }

        public Role? Role { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; }

        public IReadOnlyCollection<Permission> Permissions => _permissions;

        public static User Create(string name, string username, string passwordHash, Role role)
        {
            if (role == null) throw new DomainException("Role cannot be null");

            return new User(
                id: 0,
                name: name,
                username: username,
                passwordHash: passwordHash,
                role: role,
                isActive: true,
                createdAt: DateTime.UtcNow,
                roleId: role.Id
            );
        }

        public static User Load(int id, string name, string username, string passwordHash, int roleId, bool isActive, DateTime createdAt)
        {
            return new User(
                id,
                name,
                username,
                passwordHash,
                roleId,
                null,
                isActive,
                createdAt
            );
        }

        public static User LoadWithRole(int id, string name, string username, string passwordHash, Role role, bool isActive, DateTime createdAt)
        {
            return new User(
                id,
                name,
                username,
                passwordHash,
                role.Id,
                role,
                isActive,
                createdAt
            );
        }

        public void ChangeUsername(string username)
        {
            _ValidateUsername(username);

            if (Username == username)
                return;

            Username = username;
        }

        public void ChangeName(string name)
        {
            _ValidateName(name);

            if (Name == name)
                return;

            Name = name;
        }

        public void ChangeRole(Role role)
        {
            if (role is null)
            {
                throw new DomainException("Role cannot be null");
            }
            Role = role;
            RoleId = role.Id;
        }

        public void ChangePassword(string passwordHash)
        {
            _ValidatePasswordHash(passwordHash);

            if (PasswordHash == passwordHash)
                throw new DomainException("New password must be different.");

            PasswordHash = passwordHash;
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("User is already active.");

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("User is already inactive.");

            if (_IsSuperAdmin())
                throw new DomainException("Super admin cannot be deactivated.");

            IsActive = false;
        }

        public void SetPermissions(IEnumerable<Permission> permissions)
        {
            _permissions.Clear();

            foreach (var permission in permissions)
                AddPermission(permission);
        }

        public void AddPermission(Permission permission)
        {
            ArgumentNullException.ThrowIfNull(permission);

            if (_IsSuperAdmin())
                throw new DomainException("Cannot add permissions to super admin.");

            if (_IsCustomer())
                throw new DomainException("Cannot add permissions to customer.");

            if (_permissions.Any(p => p.Id == permission.Id))
                return;

            _permissions.Add(permission);
        }

        private static void _ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("Username is required.");
        }

        private static void _ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");
        }

        private static void _ValidatePasswordHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new DomainException("Password hash is required.");
        }

        private bool _IsSuperAdmin()
        {
            return RoleId == (int)RoleType.SuperAdmin;
        }

        private bool _IsCustomer()
        {
            return RoleId == (int)RoleType.Customer;
        }
    }
}
