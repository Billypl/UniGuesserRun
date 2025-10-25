using System.ComponentModel.DataAnnotations;

namespace UniGuesser.Application.Models.AccountModels
{
    public class RegisterUserDto
    {
        [Required]
        public string Nickname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required] public string Role { get; set; } = "User";

    }
}
