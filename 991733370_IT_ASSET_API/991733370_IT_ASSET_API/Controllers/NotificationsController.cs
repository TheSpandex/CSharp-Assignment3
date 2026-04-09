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
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public NotificationsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST /api/notifications/generate-overdue — ITAdmin only
        [HttpPost("generate-overdue")]
        [Authorize(Roles = "ITAdmin")]
        public async Task<IActionResult> GenerateOverdue()
        {
            // find all active loans that are past their expected return date
            var overdueLoans = await _context.Loans
                .Include(l => l.Equipment)
                .Where(l => !l.IsReturned && l.ExpectedReturnDate < DateTime.Now)
                .ToListAsync();

            var created = new List<string>();

            foreach (var loan in overdueLoans)
            {
                // prevent duplicate notifications for the same loan
                bool alreadyNotified = await _context.Notifications
                    .AnyAsync(n => n.LoanId == loan.LoanId);

                if (alreadyNotified) continue;

                var message = $"Overdue: '{loan.Equipment?.DeviceName}' (Asset: {loan.Equipment?.AssetTag}) was due on {loan.ExpectedReturnDate:MMM dd, yyyy}.";

                var notification = new Notification
                {
                    UserId = loan.UserId,
                    LoanId = loan.LoanId,
                    Message = message,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
                created.Add(message);
            }

            await _context.SaveChangesAsync();

            return Ok(new GenerateOverdueResponse
            {
                NotificationsCreated = created.Count,
                Messages = created
            });
        }

        // GET /api/notifications/my-alerts
        [HttpGet("my-alerts")]
        public async Task<IActionResult> GetMyAlerts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var notifications = await _context.Notifications
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // PUT /api/notifications/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            // users can only mark their own notifications
            if (notification.UserId != user.Id) return Forbid();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification marked as read." });
        }
    }
}
