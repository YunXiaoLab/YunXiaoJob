using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.JobPostings;
using YunXiaoJob.Application.UseCases.Queries;
namespace YunXiaoJob.API.Controllers;
[Route("api/job-postings")]
public class JobPostingsController : ApiControllerBase
{
    [HttpGet] public async Task<IActionResult> Search([FromQuery] string? keyword, [FromQuery] string? location, [FromQuery] YunXiaoJob.Domain.Enums.EmploymentType? employmentType, [FromQuery] YunXiaoJob.Domain.Enums.WorkplaceType? workplaceType, [FromServices] SearchJobPostingsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(new SearchJobPostingsRequest(keyword, location, employmentType, workplaceType), ct));
    [Authorize, HttpPost] public async Task<IActionResult> Create(CreateJobPostingRequest request, [FromServices] CreateJobPostingUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [Authorize, HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateJobPostingRequest request, [FromServices] UpdateJobPostingUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [Authorize, HttpPost("{id:guid}/submit")] public async Task<IActionResult> Submit(Guid id, [FromServices] SubmitJobPostingUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [Authorize, HttpGet("company/{companyId:guid}")] public async Task<IActionResult> CompanyJobs(Guid companyId, [FromServices] GetCompanyJobPostingsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(companyId, CurrentUserId, ct));
}
