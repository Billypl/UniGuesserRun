namespace UniGuesser.Application.Models.AccountModels;

public class AccountDetailsDto
{
    public string Guid { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}