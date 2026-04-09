using _991733370_IT_ASSET_API.Data;
using _991733370_IT_ASSET_API.DTOs;
using _991733370_IT_ASSET_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _991733370_IT_ASSET_API.Controllers
{
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST /api/checkout/{equipmentId}
        [HttpPost("api/checkout/{equipmentId}")]
        public async Task<IActionResult> Checkout(int equipmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null) return NotFound(new { message = "Equipment not found." });
            if (!equipment.IsAvailable) return BadRequest(new { message = "Equipment is not available." });

            var loan = new Loan
            {
                UserId = user.Id,
                EquipmentId = equipmentId,
                CheckoutDate = DateTime.Now,
                ExpectedReturnDate = DateTime.Now.AddDays(7)
            };

            equipment.IsAvailable = false;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return Ok(new LoanResponse
            {
                LoanId = loan.LoanId,
                UserId = user.Id,
                UserEmail = user.Email ?? string.Empty,
                EquipmentId = equipment.EquipmentId,
                DeviceName = equipment.DeviceName,
                AssetTag = equipment.AssetTag,
                CheckoutDate = loan.CheckoutDate,
                ExpectedReturnDate = loan.ExpectedReturnDate,
                IsReturned = false
            });
        }

        // POST /api/checkin/{loanId}
        [HttpPost("api/checkin/{loanId}")]
        public async Task<IActionResult> Checkin(int loanId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var loan = await _context.Loans
                .Include(l => l.Equipment)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null) return NotFound(new { message = "Loan not found." });
            if (loan.IsReturned) return BadRequest(new { message = "Asset already returned." });

            // employees can only check in their own loans
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("ITAdmin") && loan.UserId != user.Id)
                return Forbid();

            loan.IsReturned = true;
            loan.ReturnDate = DateTime.Now;

            if (loan.Equipment != null)
                loan.Equipment.IsAvailable = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Asset returned successfully.", returnDate = loan.ReturnDate });
        }
    }
}
