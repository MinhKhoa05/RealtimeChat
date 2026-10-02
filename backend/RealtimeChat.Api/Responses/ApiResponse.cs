namespace RealtimeChat.Api.Responses;

public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public object? Data { get; init; }

    public static ApiResponse Ok(object? data = null, string? message = null)
        => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    public static ApiResponse Error(string message)
        => new()
        {
            Success = false,
            Message = message
        };
}