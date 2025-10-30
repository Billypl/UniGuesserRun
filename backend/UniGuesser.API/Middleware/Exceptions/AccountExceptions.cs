namespace UniGuesser.API.Middleware.Exceptions;

public class AccountExceptions
{
    public class NicknameUsedException(string nick) :
        BaseException($"NicknameOrEmail {nick} already used")
    {
        public override int StatusCode => 404;
    }

    public class EmailUsedException(string email) :
        BaseException($"Email {email} already used")
    {
        public override int StatusCode => 409;
    }

    public class InvalidUsernameOrPasswordException() :
        BaseException("Username or password is invalid")
    {
        public override int StatusCode => 401;
    }

    public class UserNotFoundException(Guid userGuid) :
        BaseException($"User with GUID {userGuid} was not found")
    {
        public override int StatusCode => 404;
    }
}