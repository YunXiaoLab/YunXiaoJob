using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;
namespace YunXiaoJob.Application.UseCases.Candidates;
public sealed class SetDefaultResumeUseCase
{
    private readonly ICandidateRepository _candidates; private readonly IUnitOfWork _unit;
    public SetDefaultResumeUseCase(ICandidateRepository candidates, IUnitOfWork unit) { _candidates = candidates; _unit = unit; }
    public async Task<ResumeResponse> ExecuteAsync(Guid resumeId, Guid userId, CancellationToken ct = default)
    { var profile = await _candidates.GetProfileByUserIdAsync(userId, true, ct) ?? throw new KeyNotFoundException("Candidate profile was not found."); var resume = profile.Resumes.FirstOrDefault(x => x.Id == resumeId) ?? throw new KeyNotFoundException("Resume was not found."); foreach (var item in profile.Resumes) item.IsDefault = item.Id == resume.Id; await _unit.SaveChangesAsync(ct); return new ResumeResponse(resume.Id, resume.Name, resume.FileUrl, true); }
}
