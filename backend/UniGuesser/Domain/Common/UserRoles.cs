using System.ComponentModel;

namespace UniGuesser.Domain.Common
{
    public enum UserRoles
    {
        [Description("User")]
        User = 1,
        [Description("Moderator")]
        Moderator = 2,
        [Description("Admin")]
        Admin = 3,
    }
}
