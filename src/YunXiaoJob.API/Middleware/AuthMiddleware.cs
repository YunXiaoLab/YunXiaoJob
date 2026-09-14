using System.IdentityModel.Tokens.Jwt;
namespace YunXiaoJob.API.Middleware;
public class AuthMiddleware
{
    private readonly RequestDelegate _next; public AuthMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        { var value = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? context.User.FindFirst("sub")?.Value; if (Guid.TryParse(value, out var userId)) context.Items["CurrentUserId"] = userId; }
        await _next(context);
    }
}
