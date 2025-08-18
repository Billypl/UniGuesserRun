using System.ComponentModel.DataAnnotations;

namespace Models.AccountModels
{
    public class LoginUserDto
    {
        [Required]
        public string NicknameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
