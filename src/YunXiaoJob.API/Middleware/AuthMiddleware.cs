using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using YunXiaoJob.Application.Interfaces.Repositories;
namespace YunXiaoJob.API.Middleware;
public class AuthMiddleware
{
    private readonly RequestDelegate _next; public AuthMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context, IUserRepository users)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // JwtBearer maps the JWT "sub" claim to NameIdentifier by default.
            // Support both mapped and unmapped claims so authenticated requests always receive their user id.
            var value = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? context.User.FindFirst("sub")?.Value;
            if (Guid.TryParse(value, out var userId))
            {
                var user = await users.GetByIdAsync(userId, context.RequestAborted);
                if (user?.IsActive == true) context.Items["CurrentUserId"] = userId;
                else context.User = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
        await _next(context);
    }
}
