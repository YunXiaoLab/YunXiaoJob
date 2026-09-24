using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.UseCases.Candidates;
using YunXiaoJob.Application.UseCases.Queries;
namespace YunXiaoJob.API.Controllers;
[Authorize, Route("api/candidate")]
public class CandidateController : ApiControllerBase
{
    [HttpGet("profile")] public async Task<IActionResult> Profile([FromServices] GetMyCandidateProfileUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpPut("profile")] public async Task<IActionResult> Update(UpdateCandidateProfileRequest request, [FromServices] UpdateCandidateProfileUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPost("resumes")] public async Task<IActionResult> AddResume(AddResumeRequest request, [FromServices] AddResumeUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPut("resumes/{resumeId:guid}/default")] public async Task<IActionResult> SetDefaultResume(Guid resumeId, [FromServices] SetDefaultResumeUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(resumeId, CurrentUserId, ct));
    [HttpGet("saved-jobs")] public async Task<IActionResult> SavedJobs([FromServices] GetSavedJobsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
    [HttpPost("saved-jobs/{jobId:guid}")] public async Task<IActionResult> SaveJob(Guid jobId, [FromServices] SaveJobUseCase useCase, CancellationToken ct) { await useCase.ExecuteAsync(jobId, CurrentUserId, ct); return OkData(new { }); }
    [HttpDelete("saved-jobs/{jobId:guid}")] public async Task<IActionResult> RemoveSavedJob(Guid jobId, [FromServices] RemoveSavedJobUseCase useCase, CancellationToken ct) { await useCase.ExecuteAsync(jobId, CurrentUserId, ct); return OkData(new { }); }
    [HttpPut("educations")] public async Task<IActionResult> Educations(UpdateCandidateEducationsRequest request, [FromServices] UpdateCandidateEducationsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPut("experiences")] public async Task<IActionResult> Experiences(UpdateCandidateExperiencesRequest request, [FromServices] UpdateCandidateExperiencesUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpPut("skills")] public async Task<IActionResult> Skills(UpdateCandidateSkillsRequest request, [FromServices] UpdateCandidateSkillsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(request, CurrentUserId, ct));
    [HttpGet("applications")] public async Task<IActionResult> Applications([FromServices] GetMyApplicationsUseCase useCase, CancellationToken ct) => OkData(await useCase.ExecuteAsync(CurrentUserId, ct));
}
