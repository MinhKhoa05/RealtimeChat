using RealtimeChat.BLL.Exceptions;

namespace RealtimeChat.Api;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ErrorCode? ErrorCode { get; set; }

    public static ApiResponse Ok() => new()
    {
        Success = true
    };

    public static ApiResponse<T> Ok<T>(T? data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse Fail(ErrorCode errorCode, string? message = null) => new()
    {
        Success = false,
        Message = message ?? errorCode.GetMessage(),
        ErrorCode = errorCode
    };

    public static ApiResponse<T> Fail<T>(ErrorCode errorCode, string? message = null, T? data = default) => new()
    {
        Success = false,
        Message = message ?? errorCode.GetMessage(),
        ErrorCode = errorCode,
        Data = data
    };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}
