using _991733370_ASSET_CLIENT.Models;
using _991733370_ASSET_CLIENT.Services;
using Microsoft.AspNetCore.Mvc;

namespace _991733370_ASSET_CLIENT.Controllers
{
    public class LoansController : Controller
    {
        private readonly AssetApiService _api;

        public LoansController(AssetApiService api) => _api = api;

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("IsLoggedIn") == "true";

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "ITAdmin";

        // GET /Loans/MyLoans
        public async Task<IActionResult> MyLoans()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var loans = await _api.GetMyLoansAsync();
            if (loans is null)
            {
                TempData["Error"] = "Could not retrieve your loans.";
                return View(new List<LoanResponse>());
            }
            return View(loans);
        }

        // GET /Loans/AllLoans  (ITAdmin only)
        public async Task<IActionResult> AllLoans()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("MyLoans"); }

            var loans = await _api.GetAllLoansAsync();
            if (loans is null)
            {
                TempData["Error"] = "Could not retrieve loans.";
                return View(new List<LoanResponse>());
            }
            return View(loans);
        }

        // POST /Loans/Checkin/5
        [HttpPost]
        public async Task<IActionResult> Checkin(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var response = await _api.CheckinAsync(id);
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = AssetApiService.FriendlyError(response);
            else
                TempData["Success"] = "Asset returned successfully.";

            return RedirectToAction("MyLoans");
        }
    }
}
