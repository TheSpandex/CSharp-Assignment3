using _991733370_ASSET_CLIENT.Models;
using _991733370_ASSET_CLIENT.Services;
using Microsoft.AspNetCore.Mvc;

namespace _991733370_ASSET_CLIENT.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AssetApiService _api;

        public ProfileController(AssetApiService api) => _api = api;

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("IsLoggedIn") == "true";

        // GET /Profile
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var user = await _api.GetMeAsync();
            if (user is null) { TempData["Error"] = "Could not load profile."; return RedirectToAction("Index", "Home"); }

            ViewBag.User = user;
            return View(new UpdateUserViewModel());
        }

        // POST /Profile
        [HttpPost]
        public async Task<IActionResult> Index(UpdateUserViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var response = await _api.UpdateUserAsync(model);

            if (!response.IsSuccessStatusCode)
            {
                var body = await _api.ReadErrorAsync(response);
                ViewBag.Error = string.IsNullOrWhiteSpace(body)
                    ? AssetApiService.FriendlyError(response)
                    : body;
                ViewBag.User = await _api.GetMeAsync();
                return View(model);
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }
    }
}
