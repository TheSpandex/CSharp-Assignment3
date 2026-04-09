using _991733370_ASSET_CLIENT.Models;
using _991733370_ASSET_CLIENT.Services;
using Microsoft.AspNetCore.Mvc;

namespace _991733370_ASSET_CLIENT.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly AssetApiService _api;

        public EquipmentController(AssetApiService api) => _api = api;

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("IsLoggedIn") == "true";

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "ITAdmin";

        // GET /Equipment
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var list = await _api.GetEquipmentAsync();
            if (list is null)
            {
                TempData["Error"] = "Unable to retrieve equipment list.";
                return View(new List<EquipmentResponse>());
            }
            return View(list);
        }

        // GET /Equipment/Create  (ITAdmin only)
        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("Index"); }
            return View();
        }

        // POST /Equipment/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateEquipmentViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("Index"); }

            if (!ModelState.IsValid) return View(model);

            var response = await _api.CreateEquipmentAsync(model);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = AssetApiService.FriendlyError(response);
                return View(model);
            }

            TempData["Success"] = $"Equipment '{model.DeviceName}' added successfully.";
            return RedirectToAction("Index");
        }

        // GET /Equipment/Edit/5  (ITAdmin only)
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("Index"); }

            var item = await _api.GetEquipmentByIdAsync(id);
            if (item is null) return NotFound();

            var vm = new UpdateEquipmentViewModel
            {
                EquipmentId = item.EquipmentId,
                AssetTag = item.AssetTag,
                DeviceName = item.DeviceName,
                Description = item.Description,
                IsAvailable = item.IsAvailable
            };
            return View(vm);
        }

        // POST /Equipment/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateEquipmentViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("Index"); }

            var response = await _api.UpdateEquipmentAsync(id, model);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = AssetApiService.FriendlyError(response);
                return View(model);
            }

            TempData["Success"] = "Equipment updated successfully.";
            return RedirectToAction("Index");
        }

        // GET /Equipment/Delete/5  (ITAdmin only)
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("Index"); }

            var item = await _api.GetEquipmentByIdAsync(id);
            if (item is null) return NotFound();
            return View(item);
        }

        // POST /Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) { TempData["Error"] = "Access Denied."; return RedirectToAction("Index"); }

            var response = await _api.DeleteEquipmentAsync(id);
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = AssetApiService.FriendlyError(response);
            else
                TempData["Success"] = "Equipment deleted successfully.";

            return RedirectToAction("Index");
        }

        // POST /Equipment/Checkout/5
        [HttpPost]
        public async Task<IActionResult> Checkout(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var response = await _api.CheckoutAsync(id);
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = AssetApiService.FriendlyError(response);
            else
                TempData["Success"] = "Asset checked out successfully. Please return it by the due date.";

            return RedirectToAction("Index");
        }
    }
}
