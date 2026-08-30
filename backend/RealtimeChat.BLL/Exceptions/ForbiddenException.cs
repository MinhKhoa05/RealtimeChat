namespace RealtimeChat.BLL.Exceptions;

public class ForbiddenException : BusinessException
{
    public ForbiddenException(ErrorCode errorCode = ErrorCode.Forbidden, string? message = null)
        : base(errorCode, message)
    {
    }
}
