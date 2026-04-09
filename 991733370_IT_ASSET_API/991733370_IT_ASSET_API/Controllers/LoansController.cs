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
    [Route("api/loans")]
    [Authorize]
    public class LoansController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LoansController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET /api/loans/my-loans
        [HttpGet("my-loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var loans = await _context.Loans
                .Include(l => l.Equipment)
                .Where(l => l.UserId == user.Id)
                .Select(l => MapToResponse(l, user.Email ?? string.Empty))
                .ToListAsync();

            return Ok(loans);
        }

        // GET /api/loans/all — ITAdmin only
        [HttpGet("all")]
        [Authorize(Roles = "ITAdmin")]
        public async Task<IActionResult> GetAllLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Equipment)
                .Include(l => l.User)
                .ToListAsync();

            var result = loans.Select(l => MapToResponse(l, l.User?.Email ?? string.Empty));
            return Ok(result);
        }

        private static LoanResponse MapToResponse(Loan l, string email) => new LoanResponse
        {
            LoanId = l.LoanId,
            UserId = l.UserId,
            UserEmail = email,
            EquipmentId = l.EquipmentId,
            DeviceName = l.Equipment?.DeviceName ?? string.Empty,
            AssetTag = l.Equipment?.AssetTag ?? string.Empty,
            CheckoutDate = l.CheckoutDate,
            ExpectedReturnDate = l.ExpectedReturnDate,
            IsReturned = l.IsReturned,
            ReturnDate = l.ReturnDate
        };
    }
}
