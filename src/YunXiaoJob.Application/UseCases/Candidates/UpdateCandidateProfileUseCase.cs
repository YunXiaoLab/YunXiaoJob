using YunXiaoJob.Application.DTOs.Requests;
using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces;
using YunXiaoJob.Application.Interfaces.Repositories;

namespace YunXiaoJob.Application.UseCases.Candidates;

public class UpdateCandidateProfileUseCase
{
    private readonly ICandidateRepository _candidates;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCandidateProfileUseCase(ICandidateRepository candidates, IUnitOfWork unitOfWork)
    { _candidates = candidates; _unitOfWork = unitOfWork; }

    public async Task<CandidateProfileResponse> ExecuteAsync(UpdateCandidateProfileRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        profile.Headline = request.Headline?.Trim();
        profile.Summary = request.Summary?.Trim();
        profile.Location = request.Location?.Trim();
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.IsSearchable = request.IsSearchable;
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CandidateProfileResponse(profile.Id, profile.UserId, profile.Headline, profile.Summary,
            profile.Location, profile.YearsOfExperience, profile.IsSearchable,
            profile.Resumes.Select(x => new ResumeResponse(x.Id, x.Name, x.FileUrl, x.IsDefault)).ToList());
    }
}
