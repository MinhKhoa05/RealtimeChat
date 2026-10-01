namespace RealtimeChat.Application.Exceptions;

public class DuplicateKeyException : Exception
{
    public DuplicateKeyException(Exception innerException)
        : base("A duplicate key was detected.", innerException)
    {
    }
}