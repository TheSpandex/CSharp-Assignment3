using System.ComponentModel.DataAnnotations;

namespace _991733370_IT_ASSET_API.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        [Required]
        public string AssetTag { get; set; } = string.Empty;

        [Required]
        public string DeviceName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
