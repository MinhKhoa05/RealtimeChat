using RealtimeChat.Api;
using RealtimeChat.BLL.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RealtimeChat.Api.Middlewares;

public static class ModelStateExtensions
{
    public static ApiResponse<Dictionary<string, string?>> ToApiResponse(this ModelStateDictionary modelState)
    {
        var firstError = modelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
        var firstErrorMessage = firstError?.ErrorMessage;
        if (string.IsNullOrWhiteSpace(firstErrorMessage))
        {
            firstErrorMessage = firstError?.Exception?.Message ?? "Invalid request payload.";
        }

        var errorsDetail = modelState
            .Where(entry => entry.Value!.Errors.Any())
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => !string.IsNullOrEmpty(error.ErrorMessage) ? error.ErrorMessage : error.Exception?.Message).First());

        return ApiResponse.Fail(ErrorCode.BadRequest, firstErrorMessage, errorsDetail);
    }
}
