﻿using System.ComponentModel.DataAnnotations;

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
