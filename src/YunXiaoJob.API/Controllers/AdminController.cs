using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.UseCases.Administration;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/admin")]
public class AdminController : ApiControllerBase
{
    [HttpGet("users")] public async Task<IActionResult> Users([FromServices] GetUsersUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpPut("users/{id:guid}/active")] public async Task<IActionResult> Active(Guid id, [FromQuery] bool isActive, [FromServices] SetUserActiveUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, isActive, CurrentUserId, ct));
    [HttpPost("companies/{id:guid}/approve")] public async Task<IActionResult> ApproveCompany(Guid id, [FromServices] ApproveCompanyUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [HttpPost("job-postings/{id:guid}/approve")] public async Task<IActionResult> ApproveJob(Guid id, [FromServices] ApproveJobPostingUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
}
