namespace _991733370_IT_ASSET_API.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // used to prevent duplicate notifications for the same loan
        public int LoanId { get; set; }
    }
}
