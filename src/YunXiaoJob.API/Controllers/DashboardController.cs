using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.UseCases.Dashboards;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/dashboard")]
public class DashboardController : ApiControllerBase
{
    [HttpGet("platform")] public async Task<IActionResult> Platform([FromServices] GetPlatformDashboardUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpGet("company/{companyId:guid}")] public async Task<IActionResult> Company(Guid companyId, [FromServices] GetCompanyDashboardUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(companyId, CurrentUserId, ct));
}
