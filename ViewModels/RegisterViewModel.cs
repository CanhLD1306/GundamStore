using System.ComponentModel.DataAnnotations;

namespace GundamStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8)]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\w\d\s]).{8,}")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [StringLength(20)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
