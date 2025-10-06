namespace UniGuesser.Domain.Middleware.Exceptions
{
    public abstract class BaseException(string message) : Exception(message)
    {
        public abstract int StatusCode { get; }
    }

}
