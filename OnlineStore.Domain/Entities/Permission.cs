using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.Entities
{
    public class Permission
    {
        public int Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;

        private Permission(int id, string code, string name)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new DomainException("Permission code cannot be null or empty");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new DomainException("Permission name cannot be null or empty");
            }

            Id = id;
            Code = code;
            Name = name;
        }

        public static Permission Create(string code, string name)
        {
            return new Permission(-1, code, name);
        }

        public static Permission Load(int id, string code, string name)
        {
            return new Permission(id, code, name);
        }
    }
}
