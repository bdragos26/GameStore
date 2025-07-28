using System.ComponentModel.DataAnnotations;

namespace GameStore.Shared.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50)]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}
