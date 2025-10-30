namespace UniGuesser.Domain.Exceptions;

public class GameExceptions
{
    public class EmptyNicknameException()
        : BaseException("Unlogged user must input nickname")
    {
        public override int StatusCode => 400;
    }

    public class WrongGameModeException(string gameMode)
        : BaseException($"Game mode {gameMode} is not supported for this mode")
    {
        public override int StatusCode => 400;
    }
}