using System.ComponentModel.DataAnnotations;

namespace IdentityExample.Models
{
    public class User
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Display(Name = "Student/Faculty ID")]
        public string? StudentId { get; set; }

        [Required]
        [Display(Name = "Program Name")]
        public string? ProgramName { get; set; }
    }
}
