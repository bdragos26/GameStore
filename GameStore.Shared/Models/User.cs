using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(250)]
        public string PasswordHash { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required, StringLength(20)]
        public Roles Role { get; set; } = Roles.User;
    }
}