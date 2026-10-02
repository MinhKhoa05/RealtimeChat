using System.Net;
using RealtimeChat.Api.Responses;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            DomainException => HttpStatusCode.BadRequest,

            BadRequestException => HttpStatusCode.BadRequest,
            ConflictException => HttpStatusCode.Conflict,
            ForbiddenException => HttpStatusCode.Forbidden,
            NotFoundException => HttpStatusCode.NotFound,
            UnauthorizedException => HttpStatusCode.Unauthorized,

            _ => HttpStatusCode.InternalServerError,
        };

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "Internal server error."
            : exception.Message;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        
        var response = ApiResponse.Error(message);

        await context.Response.WriteAsJsonAsync(response);
    }
}