using System.ComponentModel;

namespace UniGuesser.Domain.ValueObjects;

public enum UserRoles
{
    [Description("User")] User = 1,
    [Description("Moderator")] Moderator = 2,
    [Description("Admin")] Admin = 3
}