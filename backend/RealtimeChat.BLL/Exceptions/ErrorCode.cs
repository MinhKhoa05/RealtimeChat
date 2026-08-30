using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RealtimeChat.BLL.Exceptions;

public enum ErrorCode
{
    // System
    [Display(Name = "System error. Please try again later.")]
    InternalServerError,

    [Display(Name = "Invalid request.")]
    BadRequest,

    [Display(Name = "Data not found.")]
    NotFound,

    [Display(Name = "Session expired or invalid.")]
    Unauthorized,

    [Display(Name = "You do not have permission to perform this action.")]
    Forbidden,

    // Authentication
    [Display(Name = "Email or password is incorrect.")]
    InvalidCredentials,

    // Resources
    [Display(Name = "User not found.")]
    UserNotFound,

    // Media
    [Display(Name = "Invalid image file.")]
    InvalidImageFile
}

public static class ErrorCodeExtensions
{
    public static string GetMessage(this ErrorCode errorCode)
    {
        var attribute = errorCode.GetType()
            .GetMember(errorCode.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>();

        return attribute?.Name ?? errorCode.ToString();
    }
}
