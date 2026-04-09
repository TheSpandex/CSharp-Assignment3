using _991733370_ASSET_CLIENT.Models;
using _991733370_ASSET_CLIENT.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace _991733370_ASSET_CLIENT.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AssetApiService _api;
        private static readonly JsonSerializerOptions _json =
            new() { PropertyNameCaseInsensitive = true };

        public NotificationsController(AssetApiService api) => _api = api;

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("IsLoggedIn") == "true";

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "ITAdmin";

        // GET /Notifications/MyAlerts
        public async Task<IActionResult> MyAlerts()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var alerts = await _api.GetMyAlertsAsync();
            if (alerts is null)
            {
                TempData["Error"] = "Could not retrieve notifications.";
                return View(new List<NotificationResponse>());
            }
            return View(alerts);
        }

        // POST /Notifications/MarkRead/5
        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var response = await _api.MarkReadAsync(id);
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = AssetApiService.FriendlyError(response);
            else
                TempData["Success"] = "Notification marked as read.";

            return RedirectToAction("MyAlerts");
        }

        // POST /Notifications/GenerateOverdue  (ITAdmin only)
        [HttpPost]
        public async Task<IActionResult> GenerateOverdue()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("MyAlerts"); }

            var response = await _api.GenerateOverdueAsync();

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = AssetApiService.FriendlyError(response);
                return RedirectToAction("MyAlerts");
            }

            var result = await response.Content.ReadFromJsonAsync<GenerateOverdueResponse>(_json);
            TempData["OverdueResult"] = result is not null
                ? $"{result.NotificationsCreated} overdue notification(s) generated."
                : "Overdue generation complete.";

            if (result?.Messages.Count > 0)
                TempData["OverdueMessages"] = string.Join("|", result.Messages);

            return RedirectToAction("MyAlerts");
        }
    }
}
