namespace RealtimeChat.BLL.Exceptions;

public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(ErrorCode errorCode = ErrorCode.Unauthorized, string? message = null)
        : base(errorCode, message)
    {
    }
}
