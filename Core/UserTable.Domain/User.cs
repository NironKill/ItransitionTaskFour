using Microsoft.AspNetCore.Identity;

namespace UserTable.Domain
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int LastLoginTime { get; set; }
        public bool LockoutEnabled { get; set; } = false;
    }
}
