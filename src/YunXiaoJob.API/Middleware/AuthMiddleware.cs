using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace YunXiaoJob.API.Middleware;
public class AuthMiddleware
{
    private readonly RequestDelegate _next; public AuthMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // JwtBearer maps the JWT "sub" claim to NameIdentifier by default.
            // Support both mapped and unmapped claims so authenticated requests always receive their user id.
            var value = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? context.User.FindFirst("sub")?.Value;
            if (Guid.TryParse(value, out var userId)) context.Items["CurrentUserId"] = userId;
        }
        await _next(context);
    }
}
