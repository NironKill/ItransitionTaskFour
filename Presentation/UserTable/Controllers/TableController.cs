using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserTable.Application.DTOs;
using UserTable.Application.Repositories.Interfaces;
using UserTable.Application.Services.Interfaces;
using UserTable.Models;
using UserTable.Models.Requests;

namespace UserTable.Controllers
{
    [Route("[controller]")]
    public class TableController : Controller
    {
        private readonly IUserRepository _user;
        private readonly IAccessTokenService _accessToken;

        public TableController(IUserRepository userRepository, IAccessTokenService accessToken)
        {
            _user = userRepository;
            _accessToken = accessToken;
        }

        [HttpGet("Manage")]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            bool isInvalid = await _accessToken.ValidateToken(userIdClaim.Subject.Name);
            if (!isInvalid)
                return RedirectToAction("Login", "Account");

            UserDTO userDTO = await _user.GetByEmail(userIdClaim.Subject.Name);
            if (userDTO.LockoutEnabled)
                return RedirectToAction("Login", "Account");

            ICollection<UserDTO> dtos = await _user.GetAll();

            List<UserModel> models = new List<UserModel>();

            foreach (UserDTO dto in dtos)
            {
                UserModel model = new UserModel()
                {
                    Email = dto.Email,
                    LastLoginTime = dto.LastLoginTime,
                    Name = dto.Name,
                    LockoutEnabled = dto.LockoutEnabled
                };
                models.Add(model);
            }
            return View(models);
        }

        [HttpDelete("Delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody]UserRequest request)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            bool isInvalid = await _accessToken.ValidateToken(userIdClaim.Subject.Name);
            if (!isInvalid)
                return RedirectToAction("Login", "Account");

            UserDTO dto = await _user.GetByEmail(userIdClaim.Subject.Name);
            if (dto.LockoutEnabled)
                return RedirectToAction("Login", "Account");

            if (request.Emails.Count == default)
                return BadRequest();

            await _user.Remove(request.Emails);

            return Ok();
        }

        [HttpPost("Lock")]
        [Authorize]
        public async Task<IActionResult> Lock([FromBody]UserRequest request)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            bool isInvalid = await _accessToken.ValidateToken(userIdClaim.Subject.Name);
            if (!isInvalid)
                return RedirectToAction("Login", "Account");

            UserDTO dto = await _user.GetByEmail(userIdClaim.Subject.Name);
            if (dto.LockoutEnabled)
                return RedirectToAction("Login", "Account");

            if (request.Emails.Count == default)
                return BadRequest();

            await _user.Lock(request.Emails);

            return Ok();
        }
        [HttpPost("Unlock")]
        [Authorize]
        public async Task<IActionResult> Unlock([FromBody]UserRequest request)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            bool isInvalid = await _accessToken.ValidateToken(userIdClaim.Subject.Name);
            if (!isInvalid)
                return RedirectToAction("Login", "Account");

            UserDTO dto = await _user.GetByEmail(userIdClaim.Subject.Name);
            if (dto.LockoutEnabled)
                return RedirectToAction("Login", "Account");

            if (request.Emails.Count == default)
                return BadRequest();

            await _user.Unlock(request.Emails);

            return Ok();
        }
    }
}
