using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealtimeChat.Api.Responses;

namespace RealtimeChat.Api.Filters;

public sealed class ApiResponseFilter : IAsyncResultFilter
{
    // Tự động bọc response của Controller vào ApiResponse.
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult result && result.Value is not ApiResponse)
        {
            result.Value = ApiResponse.Ok(result.Value);
        }

        await next();
    }
}