using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.DTOs
{
    public class ResetPasswordWithTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 8),
         RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
             ErrorMessage = "Password must contain at least one uppercase, one lowercase, and one number")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
