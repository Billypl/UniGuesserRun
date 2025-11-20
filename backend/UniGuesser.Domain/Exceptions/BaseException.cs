namespace UniGuesser.Domain.Exceptions;

public abstract class BaseException(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}