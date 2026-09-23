using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.UseCases.Notifications;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/notifications")]
public class NotificationsController : ApiControllerBase
{
    [HttpGet] public async Task<IActionResult> Mine([FromServices] GetMyNotificationsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpPut("{id:guid}/read")] public async Task<IActionResult> MarkRead(Guid id, [FromServices] MarkNotificationReadUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
}
