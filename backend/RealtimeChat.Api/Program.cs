using RealtimeChat.Api;
using RealtimeChat.Api.Middleware;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Api.Swagger;
using RealtimeChat.Application;
using RealtimeChat.Infrastructure;
using RealtimeChat.Infrastructure.Persistence;
using RealtimeChat.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

// DI
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

await app.Services.ApplyMigrationsAsync();

app.UseSwagger();
app.UseSwaggerUI();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();