namespace _991733370_IT_ASSET_API.DTOs
{
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
