using System.ComponentModel.DataAnnotations;

namespace Models.AccountModels
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

    }
}
