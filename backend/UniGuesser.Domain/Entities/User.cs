using System.ComponentModel.DataAnnotations;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Domain.Entities;

public class User
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public string Nickname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRoles Role { get; set; } = UserRoles.User;
}