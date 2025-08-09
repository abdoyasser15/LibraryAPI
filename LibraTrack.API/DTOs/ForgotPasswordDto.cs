﻿using System.ComponentModel.DataAnnotations;

namespace LibraTrack.API.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
