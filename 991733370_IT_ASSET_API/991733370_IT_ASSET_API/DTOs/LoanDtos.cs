namespace _991733370_IT_ASSET_API.DTOs
{
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
}
