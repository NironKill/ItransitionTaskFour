using Microsoft.AspNetCore.Mvc;
using UserTable.Application.DTOs;
using UserTable.Application.Repositories.Interfaces;
using UserTable.Application.Services.Interfaces;
using UserTable.Models;

namespace UserTable.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _account;
        private readonly IUserRepository _userRepository;

        public AccountController(IAccountService account, IUserRepository userRepository)
        {
            _account = account;
            _userRepository = userRepository;
        }

        [HttpGet("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Account/Register")]
        public async Task<IActionResult> Register(RegisterModel model, CancellationToken cancellationToken)
        {
            RegisterDTO dto = new RegisterDTO()
            {
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            if (ModelState.IsValid)
            {
                bool isRegistered = await _account.Registration(dto, cancellationToken);
                if (isRegistered)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError(string.Empty, "An error occurred while registering the user.");
            }

            return View(model);
        }

        [HttpGet("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Account/Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            LoginDTO dto = new LoginDTO()
            {
                Email = model.Email,
                Password = model.Password
            };

            if (ModelState.IsValid)
            {
                bool isLoggedIn = await _account.Login(dto);
                if (isLoggedIn)
                {
                    await _userRepository.Update(dto.Email);
                    return RedirectToAction("Manage", "Table");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _account.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
