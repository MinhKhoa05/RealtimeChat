namespace RealtimeChat.Application.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message = "Resource conflict.")
        : base(message)
    {
    }
}