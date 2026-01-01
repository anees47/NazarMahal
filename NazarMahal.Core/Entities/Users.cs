namespace NazarMahal.Core.Entities
{
    public class Users
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        public bool IsDisabled { get; private set; }
        public string? ProfilePictureUrl { get; private set; }

        // Navigation: ONE User → MANY Orders
        public ICollection<Order> Orders { get; private set; } = new List<Order>();

        private Users() { }

        public Users(int id, string name, string email, string address, bool isDisabled, string? profilePictureUrl)
        {
            Id = id;
            Name = name;
            Email = email;
            Address = address;
            IsDisabled = isDisabled;
            ProfilePictureUrl = profilePictureUrl;
        }

        public static Users CreateUser(
            int id, string name, string email, string address, bool isDisabled, string? profilePictureUrl)
        {
            return new Users(id, name, email, address, isDisabled, profilePictureUrl);
        }

        public void UpdateUserInfo(
            string name, string email, string address, bool isDisabled, string? profilePictureUrl)
        {
            Name = name;
            Email = email;
            Address = address;
            IsDisabled = isDisabled;
            ProfilePictureUrl = profilePictureUrl;
        }
    }
}
