namespace OnlineStore.Domain.Entities
{
    public class Role
    {
        public int Id { get; }
        public string Name { get; } = null!;

        private Role(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Role Create(string name)
        {
            return new Role(0, name);
        }

        public static Role Load(int id, string name)
        {
            return new Role(id, name);
        }
    }
}
