using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserTable.Application.DTOs;
using UserTable.Application.Interfaces;
using UserTable.Application.Repositories.Interfaces;
using UserTable.Domain;

namespace UserTable.Application.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(IApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ICollection<UserDTO>> GetAll() 
        {
            List<User> users = await _context.Users.ToListAsync();

            List<User> sorted = users.OrderByDescending(x => x.LastLoginTime).ToList();

            List<UserDTO> dtos = new List<UserDTO>();
            foreach (User user in sorted)
            {
                DateTime lastLoginTime = DateTime.UnixEpoch.AddSeconds(user.LastLoginTime).ToLocalTime();

                UserDTO dto = new UserDTO()
                {
                    Email = user.Email,
                    LastLoginTime = lastLoginTime,
                    Name = $"{user.FirstName} {user.LastName}",
                    LockoutEnabled = user.LockoutEnabled
                };
                dtos.Add(dto);
            }
            return dtos;
        }
        public async Task<UserDTO> GetByEmail(string email)
        {
            User user = _context.Users.FirstOrDefault(x => x.Email == email);

            UserDTO dto = new UserDTO()
            {
                Email = user.Email,
                LastLoginTime = DateTime.UnixEpoch.AddSeconds(user.LastLoginTime).ToLocalTime(),
                Name = $"{user.FirstName} {user.LastName}",
                LockoutEnabled = user.LockoutEnabled
            };

            return dto;
        }
        public async Task Lock(ICollection<string> listEmail)
        {
            foreach (string email in listEmail)
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                user.LockoutEnabled = true;
                user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.AddYears(100));

                await _userManager.UpdateAsync(user);
            }
        }
        public async Task Unlock(ICollection<string> listEmail)
        {
            foreach (string email in listEmail)
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                user.LockoutEnabled = false;
                user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow);

                await _userManager.UpdateAsync(user);
            }
        }
        public async Task Remove(ICollection<string> listEmail)
        {
            foreach (string email in listEmail)
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                await _userManager.DeleteAsync(user);
            }
        }
        public async Task Update(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            user.LastLoginTime = Convert.ToInt32(DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds);

            await _userManager.UpdateAsync(user);
        }
    }
}