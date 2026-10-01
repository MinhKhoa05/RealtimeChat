namespace RealtimeChat.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Forbidden.")
        : base(message)
    {
    }
}