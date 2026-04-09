using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Models
{
    public class AppUser : IdentityUser
    {
        public string? StudentId { get; set; }
        public string? ProgramName { get; set; }
    }
}
