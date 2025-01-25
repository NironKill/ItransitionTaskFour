using System.ComponentModel.DataAnnotations;

namespace UserTable.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Emails")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember Me")] public bool RememberMe { get; set; } = false;
    }
}
