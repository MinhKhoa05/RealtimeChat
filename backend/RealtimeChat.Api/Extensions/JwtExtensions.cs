using System.Text;
using RealtimeChat.BLL.Exceptions;
using RealtimeChat.BLL.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace RealtimeChat.Api.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = GetTokenValidationParameters(jwtOptions!);
                options.Events = GetJwtBearerEvents();
            });

        return services;
    }

    private static TokenValidationParameters GetTokenValidationParameters(JwtOptions jwtOptions)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
    }

    private static JwtBearerEvents GetJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                await WriteErrorResponse(context.HttpContext, StatusCodes.Status401Unauthorized, ErrorCode.Unauthorized);
            },
            OnAuthenticationFailed = async context =>
            {
                await WriteErrorResponse(context.HttpContext, StatusCodes.Status401Unauthorized, ErrorCode.Unauthorized);
            },
            OnForbidden = async context =>
            {
                await WriteErrorResponse(context.HttpContext, StatusCodes.Status403Forbidden, ErrorCode.Forbidden);
            }
        };
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, ErrorCode errorCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var response = ApiResponse.Fail(errorCode);
        await context.Response.WriteAsJsonAsync(response);
    }
}
