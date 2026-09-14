using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.API.Models;
namespace YunXiaoJob.API.Controllers;
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid CurrentUserId => HttpContext.Items.TryGetValue("CurrentUserId", out var value) && value is Guid id ? id : throw new UnauthorizedAccessException();
    protected OkObjectResult OkData<T>(T data) => Ok(ApiResponse<T>.Ok(data));
}
