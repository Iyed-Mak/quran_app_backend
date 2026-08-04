using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Middleware;

public class TokenRevocationMiddleware(RequestDelegate next, ILogger<TokenRevocationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jwtId = context.User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (jwtId is not null && await authService.IsTokenRevokedAsync(jwtId))
            {
                logger.LogWarning("Rejected revoked token (jti: {JwtId}) for {Method} {Path}",
                    jwtId, context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "This token has been revoked."
                });
                return;
            }
        }

        await next(context);
    }
}
