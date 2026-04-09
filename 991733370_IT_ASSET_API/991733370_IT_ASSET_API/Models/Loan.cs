namespace _991733370_IT_ASSET_API.Models
{
    public class Loan
    {
        public int LoanId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }

        public DateTime CheckoutDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }

        public bool IsReturned { get; set; } = false;
        public DateTime? ReturnDate { get; set; }
    }
}
