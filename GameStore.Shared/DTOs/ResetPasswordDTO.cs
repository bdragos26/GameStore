using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
