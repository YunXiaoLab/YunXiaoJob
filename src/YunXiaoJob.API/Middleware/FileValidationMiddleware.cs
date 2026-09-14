namespace YunXiaoJob.API.Middleware;
public class FileValidationMiddleware
{
    private readonly RequestDelegate _next; private static readonly byte[][] Signatures = [[0x25,0x50,0x44,0x46],[0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A],[0xFF,0xD8,0xFF]];
    public FileValidationMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.HasFormContentType)
        { var form = await context.Request.ReadFormAsync(context.RequestAborted); foreach (var file in form.Files) { if (file.Length == 0 || file.Length > 10 * 1024 * 1024 || !IsAllowed(file)) { context.Response.StatusCode = StatusCodes.Status400BadRequest; await context.Response.WriteAsJsonAsync(new { success = false, error = "Unsupported or invalid file." }); return; } } }
        await _next(context);
    }
    private static bool IsAllowed(IFormFile file) { using var stream = file.OpenReadStream(); var buffer = new byte[8]; var count = stream.Read(buffer); foreach (var signature in Signatures) if (count >= signature.Length && buffer.AsSpan(0, signature.Length).SequenceEqual(signature)) return true; return false; }
}
