using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserTable.Application.DTOs;
using UserTable.Application.Enums;
using UserTable.Application.Interfaces;
using UserTable.Application.Services.Interfaces;
using UserTable.Domain;

namespace UserTable.Application.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IApplicationDbContext _context;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        private readonly IAccessTokenService _accessToken;

        public AccountService(IApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager, IAccessTokenService accessToken)
        {
            _context = context;

            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;

            _accessToken = accessToken;
        }

        public async Task<bool> Registration(RegisterDTO dto, CancellationToken cancellationToken)
        {
            User user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                LastLoginTime = Convert.ToInt32(DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds)
            };

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(Role.Admin.ToString()))
                {
                    IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(Role.Admin.ToString()));
                    if (!roleResult.Succeeded)
                        return false;
                }

                await _accessToken.Create(user.Id, cancellationToken);

                IdentityResult addToRoleResult = await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
                if (!addToRoleResult.Succeeded)
                    return false;

                await _signInManager.SignInAsync(user, false);
                return true;
            }
            return false;
        }
        public async Task<bool> Login(LoginDTO dto)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                dto.RememberMe,
                false);

            return result.Succeeded;
        }
        public async Task Logout() => await _signInManager.SignOutAsync();
    }
}
