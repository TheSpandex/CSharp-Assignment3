using _991733370_ASSET_CLIENT.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace _991733370_ASSET_CLIENT.Services
{
    // Handles all HTTP calls to the IT Asset API.
    // Registered as a singleton so the shared CookieContainer keeps the login session alive.
    public class AssetApiService
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions _json =
            new() { PropertyNameCaseInsensitive = true };

        public AssetApiService(IConfiguration config)
        {
            var baseUrl = config["ApiBaseUrl"] ?? "https://localhost:7243";

            // UseCookies = true keeps the login cookie and sends it with every request
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                // Accept the dev certificate without prompting
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        }

        // --- Auth ---

        public Task<HttpResponseMessage> RegisterAsync(RegisterViewModel dto)
            => _client.PostAsJsonAsync("/api/account/register", dto);

        public Task<HttpResponseMessage> LoginAsync(LoginViewModel dto)
            => _client.PostAsJsonAsync("/api/account/login", dto);

        public Task<HttpResponseMessage> LogoutAsync()
            => _client.PostAsync("/api/account/logout", null);

        // --- Users ---

        public async Task<UserDto?> GetMeAsync()
        {
            var r = await _client.GetAsync("/api/users/me");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<UserDto>(_json);
        }

        public async Task<List<UserDto>?> GetAllUsersAsync()
        {
            var r = await _client.GetAsync("/api/users");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<UserDto>>(_json);
        }

        public Task<HttpResponseMessage> UpdateUserAsync(UpdateUserViewModel dto)
            => _client.PutAsJsonAsync("/api/users/update", dto);

        // --- Equipment ---

        public async Task<List<EquipmentResponse>?> GetEquipmentAsync()
        {
            var r = await _client.GetAsync("/api/equipment");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<EquipmentResponse>>(_json);
        }

        public async Task<EquipmentResponse?> GetEquipmentByIdAsync(int id)
        {
            var r = await _client.GetAsync($"/api/equipment/{id}");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<EquipmentResponse>(_json);
        }

        public Task<HttpResponseMessage> CreateEquipmentAsync(CreateEquipmentViewModel dto)
            => _client.PostAsJsonAsync("/api/equipment", dto);

        public Task<HttpResponseMessage> UpdateEquipmentAsync(int id, UpdateEquipmentViewModel dto)
            => _client.PutAsJsonAsync($"/api/equipment/{id}", dto);

        public Task<HttpResponseMessage> DeleteEquipmentAsync(int id)
            => _client.DeleteAsync($"/api/equipment/{id}");

        // --- Loans ---

        public Task<HttpResponseMessage> CheckoutAsync(int equipmentId)
            => _client.PostAsync($"/api/checkout/{equipmentId}", null);

        public Task<HttpResponseMessage> CheckinAsync(int loanId)
            => _client.PostAsync($"/api/checkin/{loanId}", null);

        public async Task<List<LoanResponse>?> GetMyLoansAsync()
        {
            var r = await _client.GetAsync("/api/loans/my-loans");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<LoanResponse>>(_json);
        }

        public async Task<List<LoanResponse>?> GetAllLoansAsync()
        {
            var r = await _client.GetAsync("/api/loans/all");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<LoanResponse>>(_json);
        }

        // --- Notifications ---

        public Task<HttpResponseMessage> GenerateOverdueAsync()
            => _client.PostAsync("/api/notifications/generate-overdue", null);

        public async Task<List<NotificationResponse>?> GetMyAlertsAsync()
        {
            var r = await _client.GetAsync("/api/notifications/my-alerts");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<NotificationResponse>>(_json);
        }

        public Task<HttpResponseMessage> MarkReadAsync(int id)
            => _client.PutAsync($"/api/notifications/{id}/read", null);

        // --- Helpers ---

        public async Task<string?> ReadErrorAsync(HttpResponseMessage response)
        {
            try { return await response.Content.ReadAsStringAsync(); }
            catch { return response.ReasonPhrase; }
        }

        public static string FriendlyError(HttpResponseMessage response) =>
            response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Access denied – please log in.",
                HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
                HttpStatusCode.NotFound => "The requested resource was not found.",
                HttpStatusCode.BadRequest => "Invalid request. Please check your input.",
                _ => $"An error occurred ({(int)response.StatusCode})."
            };
    }
}
