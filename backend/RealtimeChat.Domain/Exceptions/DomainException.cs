namespace RealtimeChat.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message = "Domain Exception") : base(message) { }
    }
}
