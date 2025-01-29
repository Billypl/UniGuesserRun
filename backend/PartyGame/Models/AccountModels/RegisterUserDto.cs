using System.ComponentModel.DataAnnotations;
using PartyGame.Entities;

namespace PartyGame.Models.AccountModels
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

        public string? Role { get; set; } = "User";

    }
}
