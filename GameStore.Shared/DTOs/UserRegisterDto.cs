using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
