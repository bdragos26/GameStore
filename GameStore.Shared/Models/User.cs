using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(250)]
        public string PasswordHash { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string? FirstName { get; set; }
        [Required, MaxLength(100)]
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required, StringLength(20)] 
        public string Role { get; set; } = "User";
    }
}