using YunXiaoJob.API.Models;
namespace YunXiaoJob.API.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next; private readonly ILogger<ExceptionMiddleware> _logger; public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) { _next=next;_logger=logger; }
    public async Task InvokeAsync(HttpContext context) { try { await _next(context); } catch (Exception ex) { _logger.LogError(ex, "Request failed"); var status = ex switch { UnauthorizedAccessException => StatusCodes.Status401Unauthorized, KeyNotFoundException => StatusCodes.Status404NotFound, ArgumentException or InvalidOperationException => StatusCodes.Status400BadRequest, _ => StatusCodes.Status500InternalServerError }; context.Response.StatusCode=status; await context.Response.WriteAsJsonAsync(new ApiResponse(false, status==500 ? "An unexpected error occurred." : ex.Message)); } }
}
