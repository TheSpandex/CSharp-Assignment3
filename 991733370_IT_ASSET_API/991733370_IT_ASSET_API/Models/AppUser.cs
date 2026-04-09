using Microsoft.AspNetCore.Identity;

namespace _991733370_IT_ASSET_API.Models
{
    public class AppUser : IdentityUser
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
