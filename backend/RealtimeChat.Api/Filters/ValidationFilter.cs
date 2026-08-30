using RealtimeChat.Api.Extensions;
using RealtimeChat.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RealtimeChat.Api.Filters;

public sealed class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState.ToApiResponse());
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
