namespace RealtimeChat.Domain.Exceptions;

public class ForbiddenDomainException : DomainException
{
    public ForbiddenDomainException(string message = "Forbiden Domain Exception") : base(message) { }
}