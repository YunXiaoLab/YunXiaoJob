namespace YunXiaoJob.API.Models;
public record ApiResponse<T>(bool Success, T? Data, string? Error) { public static ApiResponse<T> Ok(T data) => new(true, data, null); }
public record ApiResponse(bool Success, string? Error) { public static ApiResponse Ok() => new(true, null); }
