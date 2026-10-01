using RealtimeChat.Api;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Api.Swagger;
using RealtimeChat.Application;
using RealtimeChat.Infrastructure;
using RealtimeChat.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();