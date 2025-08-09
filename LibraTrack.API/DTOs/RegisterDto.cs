using System.ComponentModel.DataAnnotations;

namespace LibraTrack.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]{6,}$",
            ErrorMessage = "Password must be at least 6 characters long and contain only alphanumeric characters.")]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
