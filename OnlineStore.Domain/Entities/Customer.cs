using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.ValueObjs;

namespace OnlineStore.Domain.Entities
{
    public class Customer
    {
        public int Id { get; }
        public Email Email { get; private set; } = null!;
        public string? Phone { get; set; }
        public string Address { get; private set; }
        public int UserId { get; }
        public User? User { get; }

        private Customer(int id, Email email, string address, string? phone, int userId, User? user)
        {
            ValidateAddress(address);
            if (user is not null && userId != user.Id) throw new DomainException("User id mismatch");
            Id = id;
            Email = email;
            Phone = phone;
            Address = address;
            UserId = userId;
            User = user;
        }

        public static Customer Create(User user, string email, string address, string? phone = null)
        {
            if (user == null) throw new DomainException("User cannot be null");
            return new Customer(-1, new Email(email), address, phone, user.Id, user);
        }

        public static Customer Load(int Id, string email, string address, int UserId, string? phone = null, User? user = null)
        {
            return new Customer(Id, new Email(email), address, phone, UserId, user);
        }

        public void ChangeEmail(string email)
        {
            if (email == Email.ToString()) return;
            Email = new Email(email);
        }

        public void ChangePhone(string? phone)
        {
            if (Phone == phone) return;
            Phone = phone;
        }

        public void ChangeAddress(string address)
        {
            ValidateAddress(address);
            if (Address == address) return;
            Address = address;
        }

        private static void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) throw new DomainException("Address cannot be empty or null");
        }
    }
}
