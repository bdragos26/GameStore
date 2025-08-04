using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.DTOs
{
    public class UserRegisterDto
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        [Required, StringLength(100, MinimumLength = 8),
        RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, and one number")]
        public string? Password { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
    }
}
