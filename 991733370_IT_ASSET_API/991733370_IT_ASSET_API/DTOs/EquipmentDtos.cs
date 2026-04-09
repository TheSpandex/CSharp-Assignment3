using System.ComponentModel.DataAnnotations;

namespace _991733370_IT_ASSET_API.DTOs
{
    public class CreateEquipmentDto
    {
        [Required]
        public string AssetTag { get; set; } = string.Empty;

        [Required]
        public string DeviceName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateEquipmentDto
    {
        public string? AssetTag { get; set; }
        public string? DeviceName { get; set; }
        public string? Description { get; set; }
        public bool? IsAvailable { get; set; }
    }

    public class EquipmentResponse
    {
        public int EquipmentId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
    }
}
