namespace RealtimeChat.BLL.Exceptions;

public class BusinessException : Exception
{
    public ErrorCode ErrorCode { get; }

    public BusinessException(ErrorCode errorCode, string? message = null)
        : base(message ?? errorCode.GetMessage())
    {
        ErrorCode = errorCode;
    }
}
