using YunXiaoJob.Application.DTOs.Responses;
using YunXiaoJob.Application.Interfaces.Repositories;

namespace YunXiaoJob.Application.UseCases.Candidates;

public class GetMyCandidateProfileUseCase
{
    private readonly ICandidateRepository _candidates;
    public GetMyCandidateProfileUseCase(ICandidateRepository candidates) => _candidates = candidates;

    public async Task<CandidateProfileResponse> ExecuteAsync(Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _candidates.GetProfileByUserIdAsync(currentUserId, true, cancellationToken)
            ?? throw new KeyNotFoundException("Candidate profile was not found.");
        return new CandidateProfileResponse(profile.Id, profile.UserId, profile.Headline, profile.Summary,
            profile.Location, profile.YearsOfExperience, profile.IsSearchable,
            profile.Resumes.Select(x => new ResumeResponse(x.Id, x.Name, x.FileUrl, x.IsDefault)).ToList());
    }
}
