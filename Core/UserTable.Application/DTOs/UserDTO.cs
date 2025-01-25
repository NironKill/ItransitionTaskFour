namespace UserTable.Application.DTOs
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
