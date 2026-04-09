using _991733370_ASSET_CLIENT.Models;
using _991733370_ASSET_CLIENT.Services;
using Microsoft.AspNetCore.Mvc;

namespace _991733370_ASSET_CLIENT.Controllers
{
    public class AccountController : Controller
    {
        private readonly AssetApiService _api;

        public AccountController(AssetApiService api) => _api = api;

        // GET /Account/Login
        public IActionResult Login() => View();

        // POST /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _api.LoginAsync(model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            // Fetch the user profile to capture role
            var user = await _api.GetMeAsync();
            if (user is null)
            {
                ViewBag.Error = "Login succeeded but could not retrieve your profile.";
                return View(model);
            }

            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserRole",
                user.Roles.Contains("ITAdmin") ? "ITAdmin" : "Employee");

            return RedirectToAction("Index", "Home");
        }

        // GET /Account/Register
        public IActionResult Register() => View();

        // POST /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _api.RegisterAsync(model);

            if (!response.IsSuccessStatusCode)
            {
                var body = await _api.ReadErrorAsync(response);
                ViewBag.Error = string.IsNullOrWhiteSpace(body)
                    ? AssetApiService.FriendlyError(response)
                    : body;
                return View(model);
            }

            TempData["Success"] = "Registration successful – please log in.";
            return RedirectToAction("Login");
        }

        // POST /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _api.LogoutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
