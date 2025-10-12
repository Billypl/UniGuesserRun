namespace UniGuesser.API.Middleware.Exceptions
{
    public static class GameSessionExceptions
    {
        public class GameNotFoundException : BaseException
        {
            public override int StatusCode => 404;

            public GameNotFoundException(Guid guid)
                : base($"Game with guid {guid} not found")
            {
            }

            public GameNotFoundException(int gameId)
                : base($"Game with ID {gameId} not found")
            {
            }
        }

        public class GameNotFinishedException(Guid guid)

            : BaseException($"Game with guid {guid} is not finished")
        {
            public override int StatusCode => 400;
        }

        public class GameAlreadyExistsException()
            : BaseException("A session with the same id already exists.")
        {
            public override int StatusCode => 400;
        }

        public class WrongRequestedRoundException(int actualRound, int requestedRound)
            : BaseException($"Expected round {actualRound}, got {requestedRound}")
        {
            public override int StatusCode => 400;
        }

        public class RoundNumberOverflowException(int roundNumber)
            : BaseException($"Round number {roundNumber} is too high, please finish game")
        {
            public override int StatusCode => 400;
        }

        public class GameCannotBeFinishedException(int roundsLeft)
            : BaseException($"Cannot finish game. {roundsLeft} rounds left.")
        {
            public override int StatusCode => 400;
        }

        public class UserHasActiveGameSessionException(Guid guid)

            : BaseException($"User with guid {guid} has an active game session")
        {
            private Guid? playerGuid;



            public override int StatusCode => 400;
        }



    }
}
