using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Shared.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        [StringLength(50)]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
