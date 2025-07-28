using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.DTOs
{
    public class ResetPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
