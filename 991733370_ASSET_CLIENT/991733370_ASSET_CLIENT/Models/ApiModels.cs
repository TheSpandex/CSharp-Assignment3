namespace _991733370_ASSET_CLIENT.Models
{
    // ─── Auth / User ───────────────────────────────────────────────────────────
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class UpdateUserViewModel
    {
        public string? PhoneNumber { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    // ─── Equipment ─────────────────────────────────────────────────────────────
    public class EquipmentResponse
    {
        public int EquipmentId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateEquipmentViewModel
    {
        public string AssetTag { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateEquipmentViewModel
    {
        public int EquipmentId { get; set; }
        public string? AssetTag { get; set; }
        public string? DeviceName { get; set; }
        public string? Description { get; set; }
        public bool? IsAvailable { get; set; }
    }

    // ─── Loans ─────────────────────────────────────────────────────────────────
    public class LoanResponse
    {
        public int LoanId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int EquipmentId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string AssetTag { get; set; } = string.Empty;
        public DateTime CheckoutDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    // ─── Notifications ─────────────────────────────────────────────────────────
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GenerateOverdueResponse
    {
        public int NotificationsCreated { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}
