using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Applications;
using YunXiaoJob.Application.UseCases.Queries;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/applications")]
public class ApplicationsController : ApiControllerBase
{
    [HttpPost] public async Task<IActionResult> Apply(ApplyForJobRequest request, [FromServices] ApplyForJobUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPost("{id:guid}/withdraw")] public async Task<IActionResult> Withdraw(Guid id, [FromServices] WithdrawApplicationUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [HttpPut("{id:guid}/status")] public async Task<IActionResult> Status(Guid id, ChangeApplicationStatusRequest request, [FromServices] ChangeApplicationStatusUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("{id:guid}/interviews")] public async Task<IActionResult> Interview(Guid id, ScheduleInterviewRequest request, [FromServices] ScheduleInterviewUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpGet("job-posting/{jobId:guid}")] public async Task<IActionResult> ForJob(Guid jobId, [FromQuery] YunXiaoJob.Domain.Enums.ApplicationStatus? status, [FromServices] GetJobApplicationsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(jobId, status, CurrentUserId, ct));
}
