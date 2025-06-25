using System.ComponentModel.DataAnnotations;

namespace PartyGame.Models.AccountModels
{
    public class LoginUserDto
    {
        [Required]
        public string NicknameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
