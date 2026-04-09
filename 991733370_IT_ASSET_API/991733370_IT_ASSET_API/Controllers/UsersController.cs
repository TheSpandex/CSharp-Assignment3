using _991733370_IT_ASSET_API.DTOs;
using _991733370_IT_ASSET_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _991733370_IT_ASSET_API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // GET /api/users — ITAdmin only
        [HttpGet]
        [Authorize(Roles = "ITAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(MapToDto(user, roles));
            }

            return Ok(result);
        }

        // GET /api/users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(MapToDto(user, roles));
        }

        // PUT /api/users/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (dto.PhoneNumber != null)
                user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (string.IsNullOrEmpty(dto.CurrentPassword))
                    return BadRequest(new { message = "Current password is required to set a new password." });

                var passResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!passResult.Succeeded)
                    return BadRequest(passResult.Errors);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            return Ok(new { message = "Profile updated successfully." });
        }

        private static UserDto MapToDto(AppUser user, IList<string> roles) => new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            EmployeeId = user.EmployeeId,
            Department = user.Department,
            PhoneNumber = user.PhoneNumber,
            Roles = roles
        };
    }
}
