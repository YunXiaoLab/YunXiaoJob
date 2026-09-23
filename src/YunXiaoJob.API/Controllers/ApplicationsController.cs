using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Applications;
using YunXiaoJob.Application.UseCases.Queries;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/applications")]
public class ApplicationsController : ApiControllerBase
{
    [HttpGet("{id:guid}")] public async Task<IActionResult> Detail(Guid id, [FromServices] GetApplicationDetailUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [HttpPost] public async Task<IActionResult> Apply(ApplyForJobRequest request, [FromServices] ApplyForJobUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPost("{id:guid}/withdraw")] public async Task<IActionResult> Withdraw(Guid id, [FromServices] WithdrawApplicationUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, CurrentUserId, ct));
    [HttpPut("{id:guid}/status")] public async Task<IActionResult> Status(Guid id, ChangeApplicationStatusRequest request, [FromServices] ChangeApplicationStatusUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPut("{id:guid}/recruiter")] public async Task<IActionResult> AssignRecruiter(Guid id, AssignRecruiterRequest request, [FromServices] AssignRecruiterUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("{id:guid}/interviews")] public async Task<IActionResult> Interview(Guid id, ScheduleInterviewRequest request, [FromServices] ScheduleInterviewUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("interviews/{interviewId:guid}/complete")] public async Task<IActionResult> CompleteInterview(Guid interviewId, CompleteInterviewRequest request, [FromServices] CompleteInterviewUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(interviewId, request, CurrentUserId, ct));
    [HttpPost("{id:guid}/offers")] public async Task<IActionResult> CreateOffer(Guid id, CreateJobOfferRequest request, [FromServices] CreateJobOfferUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(id, request, CurrentUserId, ct));
    [HttpPost("offers/{offerId:guid}/respond")] public async Task<IActionResult> RespondToOffer(Guid offerId, RespondToJobOfferRequest request, [FromServices] RespondToJobOfferUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(offerId, request, CurrentUserId, ct));
    [HttpGet("job-posting/{jobId:guid}")] public async Task<IActionResult> ForJob(Guid jobId, [FromQuery] YunXiaoJob.Domain.Enums.ApplicationStatus? status, [FromServices] GetJobApplicationsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(jobId, status, CurrentUserId, ct));
}
