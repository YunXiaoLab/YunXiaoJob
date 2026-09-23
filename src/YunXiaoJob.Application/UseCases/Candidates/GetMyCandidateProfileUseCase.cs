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
        return profile.ToResponse();
    }
}
