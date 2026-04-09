using _991733370_ASSET_CLIENT.Models;
using _991733370_ASSET_CLIENT.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _991733370_ASSET_CLIENT.Controllers
{
    public class HomeController : Controller
    {
        private readonly AssetApiService _api;

        public HomeController(AssetApiService api) => _api = api;

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login", "Account");

            // Dashboard: show available equipment
            var equipment = await _api.GetEquipmentAsync();
            ViewBag.Role = HttpContext.Session.GetString("UserRole");
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
            return View(equipment ?? new List<EquipmentResponse>());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
