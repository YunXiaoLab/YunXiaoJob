using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Administration;
using YunXiaoJob.Application.UseCases.CompanyRegistrations;
using YunXiaoJob.Application.UseCases.Queries;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/admin")]
public class AdminController : ApiControllerBase
{
    [HttpGet("users")] public async Task<IActionResult> Users([FromServices] GetUsersUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpPut("users/{id:guid}/active")] public async Task<IActionResult> Active(Guid id, [FromQuery] bool isActive, [FromServices] SetUserActiveUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, isActive, CurrentUserId, ct));
    [HttpGet("companies")] public async Task<IActionResult> Companies([FromQuery] YunXiaoJob.Domain.Enums.CompanyStatus? status, [FromServices] GetAdminCompaniesUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(status, CurrentUserId, ct));
    [HttpGet("job-postings")] public async Task<IActionResult> JobPostings([FromQuery] YunXiaoJob.Domain.Enums.JobPostingStatus? status, [FromServices] GetAdminJobPostingsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(status, CurrentUserId, ct));
    [HttpGet("company-registrations")] public async Task<IActionResult> CompanyRegistrations([FromQuery] YunXiaoJob.Domain.Enums.CompanyRegistrationStatus? status, [FromServices] GetCompanyRegistrationsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(status, CurrentUserId, ct));
    [HttpPost("company-registrations/{id:guid}/approve")] public async Task<IActionResult> ApproveRegistration(Guid id, ReviewCompanyRegistrationRequest request, [FromServices] ApproveCompanyRegistrationUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("company-registrations/{id:guid}/reject")] public async Task<IActionResult> RejectRegistration(Guid id, ReviewCompanyRegistrationRequest request, [FromServices] RejectCompanyRegistrationUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("companies/{id:guid}/approve")] public async Task<IActionResult> ApproveCompany(Guid id, [FromServices] ApproveCompanyUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [HttpPost("job-postings/{id:guid}/approve")] public async Task<IActionResult> ApproveJob(Guid id, [FromServices] ApproveJobPostingUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
}
