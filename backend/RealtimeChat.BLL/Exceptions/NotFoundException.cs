namespace RealtimeChat.BLL.Exceptions;

public class NotFoundException : BusinessException
{
    public NotFoundException(ErrorCode errorCode = ErrorCode.NotFound, string? message = null)
        : base(errorCode, message)
    {
    }
}
